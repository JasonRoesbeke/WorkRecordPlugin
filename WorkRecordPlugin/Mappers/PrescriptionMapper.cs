/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Inge La Riviere - Initial version, based on FieldBoundaryMapper.
  *******************************************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using GeoJSON.Net.Feature;
using ADAPT.DTOs.AutoMapperProfiles;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Prescriptions;
using Newtonsoft.Json;
using WorkRecordPlugin.Mappers.GeoJson;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.UnitSystem;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Products;
using System.Collections;

namespace WorkRecordPlugin.Mappers
{
    internal class PrescriptionMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly PluginProperties _properties;
		private readonly ApplicationDataModel _dataModel;

		public PrescriptionMapper(PluginProperties properties, ApplicationDataModel dataModel = null)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			config.CreateMapper();
			_properties = properties;
			_dataModel = dataModel;
		}
		private Feature Map(Prescription prescription)
		{
			// @ToDo or not to do?
			Feature feature = JsonConvert.DeserializeObject<Feature>("{ \"type\": \"Feature\", \"properties\": {}, \"geometry\": null }");
			//feature.Properties.Add("Something", prescription.);

			return feature;
		}

		private Feature Map(ManualPrescription prescription)
		{
			// @ToDo or not to do? use field boundary?
			Feature feature = JsonConvert.DeserializeObject<Feature>("{ \"type\": \"Feature\", \"properties\": {}, \"geometry\": null }");
			feature.Properties.Add("ApplicationStrategy", prescription.ApplicationStrategy.ToString());    // Enum: RatePerArea, RatePerTank, TotalProduct
			feature.Properties.Add("ProductUses", prescription.ProductUses);					// ProductId, Rate, AppliedArea , ProductTotal
			feature.Properties.Add("TankAmount", prescription.TankAmount);
			feature.Properties.Add("TotalArea", prescription.TotalArea);
			feature.Properties.Add("TotalTanks", prescription.TotalTanks);

			return feature;
		}

		private Feature Map(VectorPrescription prescription)
		{
			Feature feature = null;
			Dictionary<string, object> properties = new Dictionary<string, object>();

			// SpatialPrescription: not sure these rates mean anything for the geojson output
			double outOfFieldRate = -1.0;   // something save to compare with
			if (prescription.OutOfFieldRate != null)
			{
				properties.Add("OutOfFieldRate", prescription.OutOfFieldRate.Value.Value);
				outOfFieldRate = prescription.OutOfFieldRate.Value.Value;
			}
			if (prescription.LossOfGpsRate != null)
				properties.Add("LossOfGpsRate", prescription.LossOfGpsRate.Value.Value);

			double minRate = double.MaxValue;
			double maxRate = -1;
			foreach (var shaperate in prescription.RxShapeLookups)
			{
				if (shaperate.Shape != null && shaperate.Shape.Polygons.Count > 0)
				{
					foreach (var rate in shaperate.Rates)
					{
						if (rate.Rate != outOfFieldRate)
						{
							minRate = Math.Min(minRate, rate.Rate);
							maxRate = Math.Max(maxRate, rate.Rate);
						}
					}
				}
				else // outoffield
					foreach (var rate in shaperate.Rates)
					{
						RxProductLookup product = prescription.RxProductLookups.Where(r => r.Id.ReferenceId == rate.RxProductLookupId).FirstOrDefault();
						if (product == null)
							Console.WriteLine("outoffield rate? " + rate.Rate + " " + rate.RxProductLookupId);
						else
							Console.WriteLine("outoffield rate? " + rate.Rate + " " + rate.RxProductLookupId + " " + product.Representation.Code);
					}
			}
			properties.Add("MinRate", minRate);
			properties.Add("MaxRate", maxRate);
			List<Dictionary<string, object>> products = new List<Dictionary<string, object>> { };
			foreach (var rxproduct in prescription.RxProductLookups)
			{
				if (products.Where(p => p.ContainsKey("productId") && (int)p["productId"] == rxproduct.ProductId).FirstOrDefault() == null)
				{
					Dictionary<string, object> product = new Dictionary<string, object>();
					product.Add("productId", rxproduct.ProductId);
					product.Add("productCode", rxproduct.Representation.Code);
					product.Add("productUom", rxproduct.UnitOfMeasure.Code);
					Product adaptProduct = _dataModel.Catalog.Products.Where(p => p.Id.ReferenceId == rxproduct.ProductId).FirstOrDefault();
					if (adaptProduct != null)
					{
						product.Add("productDescription", adaptProduct.Description);
						product.Add("productType", adaptProduct.ProductType.ToString()); // or via GetName?
					}
					products.Add(product);
				}
			}
			properties.Add("Products", products.ToArray());

			if (prescription.BoundingBox != null)
			{
				GeoJSON.Net.Geometry.Polygon polygon = PolygonMapper.MapBoundingBox(prescription.BoundingBox, _properties.AffineTransformation);  // MinX, MinY, MaxX, MaxY
				feature = new Feature(polygon, properties);
			}
			else
			{
				// @ToDo dissolve multipolygon or use Envelope?
				// NetTopologySuite.Operation.Union.CascadedPolygonUnion
				// NetTopologySuite.Algorithm.MinimumDiameter.GetMinimumRectangle
				var polygons = new List<GeoJSON.Net.Geometry.Polygon>();
				foreach (var shaperate in prescription.RxShapeLookups)
				{
					if (shaperate.Shape != null && shaperate.Shape.Polygons.Count > 0)
					{
						foreach (var adaptPolygon in shaperate.Shape.Polygons)
						{
							GeoJSON.Net.Geometry.Polygon polygon = PolygonMapper.MapPolygon(adaptPolygon, _properties.AffineTransformation);
							if (polygon != null)
							{
								polygons.Add(polygon);
							}
						}
					}
				}
				feature = new Feature(new GeoJSON.Net.Geometry.MultiPolygon(polygons), properties);
			}

			return feature;
		}
		private List<Feature> MapMultiple(VectorPrescription prescription)
		{
			List<Feature> features = new List<Feature>();

			double outOfFieldRate = -1.0;   // something save to compare with
			if (prescription.OutOfFieldRate != null)
				outOfFieldRate = prescription.OutOfFieldRate.Value.Value;
			
			foreach (var shaperate in prescription.RxShapeLookups)
            {
				int index = 0;
				if (shaperate.Shape != null && shaperate.Shape.Polygons.Count > 0)
				{
					foreach (var adaptPolygon in shaperate.Shape.Polygons)
					{
						Dictionary<string, object> properties = new Dictionary<string, object>();
						RxProductLookup product = prescription.RxProductLookups.Where(r => r.Id.ReferenceId == shaperate.Rates[index].RxProductLookupId).FirstOrDefault();
						if (product != null)
						{
							properties.Add("productId", product.ProductId);
							Product adaptProduct = _dataModel.Catalog.Products.Where(p => p.Id.ReferenceId == product.ProductId).FirstOrDefault();
							if (adaptProduct != null)
							{
								properties.Add("productDescription", adaptProduct.Description);
								properties.Add("productType", adaptProduct.ProductType.ToString());
							}
							properties.Add("productCode", product.Representation.Code);
							properties.Add("productUom",  product.UnitOfMeasure.Code);
						}
                        else
                        {
							properties.Add("productId", shaperate.Rates[index].RxProductLookupId);
						}
						properties.Add("rate", shaperate.Rates[index++].Rate);
						
						features.Add(new Feature(PolygonMapper.MapPolygon(adaptPolygon, _properties.AffineTransformation), properties));
					}
				}
			}

			return features;
		}

		private Feature Map(RasterGridPrescription prescription)
		{
			GeoJSON.Net.Geometry.IGeometryObject geometry = null;
			Dictionary<string, object> properties = new Dictionary<string, object>();
			if (prescription.BoundingBox != null)
			{
				geometry = PolygonMapper.MapBoundingBox(prescription.BoundingBox, _properties.AffineTransformation);
			}
			else
			{
				geometry = PointMapper.MapPoint2Point(prescription.Origin, _properties.AffineTransformation);
			}

			properties.Add("RowCount", prescription.RowCount);
			properties.Add("ColumnCount", prescription.ColumnCount);
			properties.Add("CellWidth", prescription.CellWidth.Value.Value);
			properties.Add("CellHeight", prescription.CellHeight.Value.Value);
			//properties.Add("RxProductLookups", prescription.RxProductLookups);  // Id, ProductId, Representation, UnitOfMeasure

			// SpatialPrescription: not sure these rates mean anything for the geojson output
			double outOfFieldRate = -1.0;   // something save to compare with
			if (prescription.OutOfFieldRate != null)
			{
				properties.Add("OutOfFieldRate", prescription.OutOfFieldRate.Value.Value);
				outOfFieldRate = prescription.OutOfFieldRate.Value.Value;
			}
			if (prescription.LossOfGpsRate != null)
				properties.Add("LossOfGpsRate", prescription.LossOfGpsRate.Value.Value);

			double minRate = double.MaxValue;
			double maxRate = -1.0;
			foreach (var rate in prescription.Rates)
			{
				if (rate.RxRates[0].Rate != outOfFieldRate)
				{
					minRate = Math.Min(minRate, rate.RxRates[0].Rate);
					maxRate = Math.Max(maxRate, rate.RxRates[0].Rate);
				}
			}
			properties.Add("MinRate", minRate);
			properties.Add("MaxRate", maxRate);

			List<Dictionary<string, object>> products = new List<Dictionary<string, object>>{};
			foreach (var rxproduct in prescription.RxProductLookups)
			{
				if (products.Where(p => p.ContainsKey("productId") && (int)p["productId"] == rxproduct.ProductId).FirstOrDefault() == null)
				{
					Dictionary<string, object> product = new Dictionary<string, object>();
					product.Add("productId", rxproduct.ProductId);
					product.Add("productCode", rxproduct.Representation.Code);
					product.Add("productUom", rxproduct.UnitOfMeasure.Code);
					Product adaptProduct = _dataModel.Catalog.Products.Where(p => p.Id.ReferenceId == rxproduct.ProductId).FirstOrDefault();
					if (adaptProduct != null)
					{
						product.Add("productDescription", adaptProduct.Description);
						product.Add("productType", adaptProduct.ProductType.ToString()); // or via GetName?
					}
					products.Add(product);
				}
			}
			//Array Values = products.Select(x => (Object)x).ToArray(); ;
			properties.Add("Products", products.ToArray());

			return new Feature(geometry, properties);
		}
		private List<Feature> MapMultiple(RasterGridPrescription prescription)
		{
			List<Feature> features = new List<Feature>();
			
			// Based on Open.Topology.TestRunner.Functions/CreateShapeFunctions.Grid
			var grid = new List<BoundingBox>();
			int nCellsOnSideX = (int)prescription.ColumnCount;
			int nCellsOnSideY = (int)prescription.RowCount;
			double cellSizeX = prescription.CellWidth.Value.Value;
			double cellSizeY = prescription.CellHeight.Value.Value;
			double dMinX, dMinY;

			if (prescription.BoundingBox != null)
			{
				dMinX = prescription.BoundingBox.MinX.Value.Value;
				dMinY = prescription.BoundingBox.MinY.Value.Value;
			}
			else
			{
				dMinX = prescription.Origin.X;
				dMinY = prescription.Origin.Y;
			}

			for (int j = 0; j < nCellsOnSideY; j++)
			{
				for (int i = 0; i < nCellsOnSideX; i++)
				{
					double x1 = dMinX + i * cellSizeX;
					double y1 = dMinY + j * cellSizeY;
					double x2 = dMinX + (i + 1) * cellSizeX;
					double y2 = dMinY + (j + 1) * cellSizeY;

					var bbox = new BoundingBox();
					bbox.MinY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), y1));
					bbox.MinX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), x1));
					bbox.MaxY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), y2));
					bbox.MaxX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), x2));
					grid.Add(bbox);
				}
			}

			int index = 0;
			double outOfFieldRate = -1.0;	// something save to compare with
			if (prescription.OutOfFieldRate != null)
				outOfFieldRate = prescription.OutOfFieldRate.Value.Value;

			Console.WriteLine($"PrescriptionMapper outOfFieldRate: {outOfFieldRate}, grids {grid.Count}");

			// @ToDo merge adjacent grid boxes with same property values?
			foreach (var bbox in grid)
			{
				// skip outOfField grids
				if (prescription.Rates[index].RxRates[0].Rate != outOfFieldRate)
                {
					GeoJSON.Net.Geometry.IGeometryObject geometry = PolygonMapper.MapBoundingBox(bbox, _properties.AffineTransformation);
					Dictionary<string, object> properties = new Dictionary<string, object>();
					RxProductLookup product = prescription.RxProductLookups.Where(r => r.Id.ReferenceId == prescription.Rates[index].RxRates[0].RxProductLookupId).FirstOrDefault();
					if (product != null)
					{
						properties.Add("productId", product.ProductId);
						Product adaptProduct = _dataModel.Catalog.Products.Where(p => p.Id.ReferenceId == product.ProductId).FirstOrDefault();
						if (adaptProduct != null)
						{
							properties.Add("productDescription", adaptProduct.Description);
							properties.Add("productType", adaptProduct.ProductType.ToString());	// or via GetName?
						}
						properties.Add("productCode", product.Representation.Code);
						properties.Add("productUom", product.UnitOfMeasure.Code);
					}
					else
					{
						properties.Add("productId", prescription.Rates[index].RxRates[0].RxProductLookupId);
					}
					properties.Add("rate", prescription.Rates[index].RxRates[0].Rate);

					features.Add(new Feature(geometry, properties));
				}
				index++;
			}

			return features;
		}

		public Feature MapAsSingleFeature(Prescription adaptPrescription)
		{
			Feature prescriptionFeature = null;

			// Prescription, types
			if (adaptPrescription is RasterGridPrescription)
			{
				prescriptionFeature = Map(adaptPrescription as RasterGridPrescription);
			}
			else if (adaptPrescription is VectorPrescription)
			{
				prescriptionFeature = Map(adaptPrescription as VectorPrescription);
			}
			else if (adaptPrescription is ManualPrescription)
			{
				// @ToDo or not to do? just log?
				prescriptionFeature = Map(adaptPrescription as ManualPrescription);
			}
			else
            {
				// @ToDo or not to do? just log?
				prescriptionFeature = Map(adaptPrescription);
			}

			if (prescriptionFeature != null)
			{
				prescriptionFeature.Properties.Add("Guid", UniqueIdMapper.GetUniqueGuid(adaptPrescription.Id, UniqueIdSourceCNH));
				Field adaptField = _dataModel.Catalog.Fields.Where(f => f.Id.ReferenceId == adaptPrescription.FieldId).FirstOrDefault();
				if (adaptField != null)
				{
					Guid fieldguid = UniqueIdMapper.GetUniqueGuid(adaptField.Id, UniqueIdSourceCNH);
					prescriptionFeature.Properties.Add("FieldId", fieldguid);

				}
				else
				{
					prescriptionFeature.Properties.Add("FieldId", adaptPrescription.FieldId);
				}

				if (_properties.Anonymise)
				{
					prescriptionFeature.Properties.Add("Description", "Prescription " + adaptPrescription.Id.ReferenceId);
				}
				else
				{
					prescriptionFeature.Properties.Add("Description", adaptPrescription.Description);
				}

				if (adaptPrescription.CropZoneId != null)
				{
					prescriptionFeature.Properties.Add("CropZoneId", adaptPrescription.CropZoneId);

					//adaptPrescription.CropZoneId
					//Prescription adaptPrescription = _dataModel.Catalog.Prescriptions.Where(f => f.Id.ReferenceId == workItemOperation.PrescriptionId).FirstOrDefault();
				}
			}
			return prescriptionFeature;
		}
		public List<Feature> MapAsMultipleFeatures(Prescription adaptPrescription)
		{
			Feature prescriptionFeature = null;
			List<Feature> prescriptionFeatures = new List<Feature>();

			// Prescription, types
			if (adaptPrescription is RasterGridPrescription)
			{
				prescriptionFeatures = MapMultiple(adaptPrescription as RasterGridPrescription);
			}
			else if (adaptPrescription is VectorPrescription)
			{
				prescriptionFeatures = MapMultiple(adaptPrescription as VectorPrescription);
			}
			else if (adaptPrescription is ManualPrescription)
			{	// @ToDo or not to do? just log?
				prescriptionFeature = Map(adaptPrescription as ManualPrescription);
			}
			else
			{	// @ToDo or not to do? just log?
				prescriptionFeature = Map(adaptPrescription);
			}

			if (prescriptionFeature != null)
			{
				prescriptionFeatures.Add(prescriptionFeature);
			}

			return prescriptionFeatures;
		}

		internal static string GetWorkItemOperationPrefix()
		{
			return "Prescription";
		}
	}
}