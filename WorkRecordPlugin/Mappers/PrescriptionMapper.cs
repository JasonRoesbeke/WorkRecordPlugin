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
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GeoJSON.Net.Feature;
using ADAPT.DTOs.AutoMapperProfiles;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Prescriptions;
using Newtonsoft.Json;
using WorkRecordPlugin.Mappers.GeoJson;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using NetTopologySuite.Utilities;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.UnitSystem;

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
			Feature feature = null;
			// @ToDo or not to do?
			feature = JsonConvert.DeserializeObject<Feature>("{ \"type\": \"Feature\", \"properties\": {}, \"geometry\": { \"type\": \"Polygon\", \"coordinates\": [ [ [ 6.057268, 52.931243 ], [ 6.057268, 52.931225 ], [ 6.057238, 52.931225 ], [ 6.057208, 52.931225 ], [ 6.057208, 52.931243 ], [ 6.057208, 52.931261 ], [ 6.057178, 52.931261 ], [ 6.057178, 52.931279 ], [ 6.057148, 52.931279 ], [ 6.057148, 52.931297 ], [ 6.057148, 52.931315 ], [ 6.057118, 52.931315 ], [ 6.057118, 52.931333 ], [ 6.057088, 52.931333 ], [ 6.057088, 52.931351 ], [ 6.057088, 52.931369 ], [ 6.057058, 52.931369 ], [ 6.057058, 52.931387 ], [ 6.057058, 52.931405 ], [ 6.057028, 52.931405 ], [ 6.057028, 52.931423 ], [ 6.056998, 52.931423 ], [ 6.056998, 52.931441 ], [ 6.056998, 52.931459 ], [ 6.056968, 52.931459 ], [ 6.056968, 52.931477 ], [ 6.056938, 52.931477 ], [ 6.056938, 52.931495 ], [ 6.056938, 52.931513 ], [ 6.056908, 52.931513 ], [ 6.056908, 52.931531 ], [ 6.056908, 52.931549 ], [ 6.056878, 52.931549 ], [ 6.056878, 52.931567 ], [ 6.056848, 52.931567 ], [ 6.056848, 52.931585 ], [ 6.056848, 52.931603 ], [ 6.056818, 52.931603 ], [ 6.056818, 52.931621 ], [ 6.056788, 52.931621 ], [ 6.056788, 52.931639 ], [ 6.056788, 52.931657 ], [ 6.056758, 52.931657 ], [ 6.056758, 52.931675 ], [ 6.056758, 52.931693 ], [ 6.056728, 52.931693 ], [ 6.056728, 52.931711 ], [ 6.056698, 52.931711 ], [ 6.056698, 52.931729 ], [ 6.056698, 52.931747 ], [ 6.056668, 52.931747 ], [ 6.056668, 52.931765 ], [ 6.056638, 52.931765 ], [ 6.056638, 52.931783 ], [ 6.056638, 52.931801 ], [ 6.056608, 52.931801 ], [ 6.056608, 52.931819 ], [ 6.056608, 52.931837 ], [ 6.056578, 52.931837 ], [ 6.056578, 52.931855 ], [ 6.056548, 52.931855 ], [ 6.056548, 52.931873 ], [ 6.056548, 52.931891 ], [ 6.056518, 52.931891 ], [ 6.056518, 52.931909 ], [ 6.056488, 52.931909 ], [ 6.056488, 52.931927 ], [ 6.056488, 52.931945 ], [ 6.056458, 52.931945 ], [ 6.056458, 52.931963 ], [ 6.056458, 52.931981 ], [ 6.056428, 52.931981 ], [ 6.056428, 52.931999 ], [ 6.056398, 52.931999 ], [ 6.056398, 52.932017 ], [ 6.056398, 52.932035 ], [ 6.056368, 52.932035 ], [ 6.056368, 52.932053 ], [ 6.056338, 52.932053 ], [ 6.056338, 52.932071 ], [ 6.056338, 52.932089 ], [ 6.056308, 52.932089 ], [ 6.056308, 52.932107 ], [ 6.056308, 52.932125 ], [ 6.056278, 52.932125 ], [ 6.056278, 52.932143 ], [ 6.056248, 52.932143 ], [ 6.056248, 52.932161 ], [ 6.056248, 52.932179 ], [ 6.056218, 52.932179 ], [ 6.056218, 52.932197 ], [ 6.056188, 52.932197 ], [ 6.056188, 52.932215 ], [ 6.056188, 52.932233 ], [ 6.056158, 52.932233 ], [ 6.056158, 52.932251 ], [ 6.056188, 52.932251 ], [ 6.056218, 52.932251 ], [ 6.056218, 52.932269 ], [ 6.056248, 52.932269 ], [ 6.056248, 52.932287 ], [ 6.056278, 52.932287 ], [ 6.056308, 52.932287 ], [ 6.056308, 52.932305 ], [ 6.056338, 52.932305 ], [ 6.056338, 52.932323 ], [ 6.056368, 52.932323 ], [ 6.056398, 52.932323 ], [ 6.056398, 52.932341 ], [ 6.056428, 52.932341 ], [ 6.056458, 52.932341 ], [ 6.056458, 52.932359 ], [ 6.056488, 52.932359 ], [ 6.056488, 52.932377 ], [ 6.056518, 52.932377 ], [ 6.056548, 52.932377 ], [ 6.056548, 52.932395 ], [ 6.056578, 52.932395 ], [ 6.056608, 52.932395 ], [ 6.056608, 52.932413 ], [ 6.056638, 52.932413 ], [ 6.056638, 52.932395 ], [ 6.056638, 52.932377 ], [ 6.056668, 52.932377 ], [ 6.056668, 52.932359 ], [ 6.056698, 52.932359 ], [ 6.056698, 52.932341 ], [ 6.056698, 52.932323 ], [ 6.056728, 52.932323 ], [ 6.056728, 52.932341 ], [ 6.056758, 52.932341 ], [ 6.056788, 52.932341 ], [ 6.056788, 52.932359 ], [ 6.056818, 52.932359 ], [ 6.056848, 52.932359 ], [ 6.056848, 52.932377 ], [ 6.056878, 52.932377 ], [ 6.056878, 52.932395 ], [ 6.056908, 52.932395 ], [ 6.056938, 52.932395 ], [ 6.056938, 52.932413 ], [ 6.056968, 52.932413 ], [ 6.056998, 52.932413 ], [ 6.056998, 52.932431 ], [ 6.057028, 52.932431 ], [ 6.057028, 52.932449 ], [ 6.057058, 52.932449 ], [ 6.057088, 52.932449 ], [ 6.057088, 52.932467 ], [ 6.057118, 52.932467 ], [ 6.057118, 52.932485 ], [ 6.057148, 52.932485 ], [ 6.057178, 52.932485 ], [ 6.057178, 52.932503 ], [ 6.057208, 52.932503 ], [ 6.057238, 52.932503 ], [ 6.057238, 52.932521 ], [ 6.057268, 52.932521 ], [ 6.057268, 52.932539 ], [ 6.057298, 52.932539 ], [ 6.057328, 52.932539 ], [ 6.057328, 52.932557 ], [ 6.057358, 52.932557 ], [ 6.057358, 52.932539 ], [ 6.057388, 52.932539 ], [ 6.057388, 52.932521 ], [ 6.057418, 52.932521 ], [ 6.057418, 52.932503 ], [ 6.057418, 52.932485 ], [ 6.057448, 52.932485 ], [ 6.057448, 52.932467 ], [ 6.057448, 52.932449 ], [ 6.057478, 52.932449 ], [ 6.057478, 52.932431 ], [ 6.057508, 52.932431 ], [ 6.057508, 52.932413 ], [ 6.057508, 52.932395 ], [ 6.057538, 52.932395 ], [ 6.057538, 52.932377 ], [ 6.057568, 52.932377 ], [ 6.057568, 52.932359 ], [ 6.057568, 52.932341 ], [ 6.057598, 52.932341 ], [ 6.057598, 52.932323 ], [ 6.057598, 52.932305 ], [ 6.057628, 52.932305 ], [ 6.057628, 52.932287 ], [ 6.057658, 52.932287 ], [ 6.057658, 52.932269 ], [ 6.057658, 52.932251 ], [ 6.057688, 52.932251 ], [ 6.057688, 52.932233 ], [ 6.057688, 52.932215 ], [ 6.057718, 52.932215 ], [ 6.057718, 52.932197 ], [ 6.057748, 52.932197 ], [ 6.057748, 52.932179 ], [ 6.057748, 52.932161 ], [ 6.057778, 52.932161 ], [ 6.057778, 52.932143 ], [ 6.057808, 52.932143 ], [ 6.057808, 52.932125 ], [ 6.057808, 52.932107 ], [ 6.057838, 52.932107 ], [ 6.057838, 52.932089 ], [ 6.057868, 52.932089 ], [ 6.057868, 52.932071 ], [ 6.057868, 52.932053 ], [ 6.057898, 52.932053 ], [ 6.057898, 52.932035 ], [ 6.057928, 52.932035 ], [ 6.057928, 52.932017 ], [ 6.057928, 52.931999 ], [ 6.057958, 52.931999 ], [ 6.057958, 52.931981 ], [ 6.057958, 52.931963 ], [ 6.057988, 52.931963 ], [ 6.057988, 52.931945 ], [ 6.058018, 52.931945 ], [ 6.058018, 52.931927 ], [ 6.058018, 52.931909 ], [ 6.058048, 52.931909 ], [ 6.058048, 52.931891 ], [ 6.058078, 52.931891 ], [ 6.058078, 52.931873 ], [ 6.058078, 52.931855 ], [ 6.058108, 52.931855 ], [ 6.058108, 52.931837 ], [ 6.058138, 52.931837 ], [ 6.058138, 52.931819 ], [ 6.058138, 52.931801 ], [ 6.058168, 52.931801 ], [ 6.058168, 52.931783 ], [ 6.058168, 52.931765 ], [ 6.058198, 52.931765 ], [ 6.058198, 52.931747 ], [ 6.058228, 52.931747 ], [ 6.058228, 52.931729 ], [ 6.058228, 52.931711 ], [ 6.058258, 52.931711 ], [ 6.058258, 52.931693 ], [ 6.058258, 52.931675 ], [ 6.058288, 52.931675 ], [ 6.058288, 52.931657 ], [ 6.058318, 52.931657 ], [ 6.058318, 52.931639 ], [ 6.058318, 52.931621 ], [ 6.058318, 52.931603 ], [ 6.058348, 52.931603 ], [ 6.058348, 52.931585 ], [ 6.058378, 52.931585 ], [ 6.058378, 52.931567 ], [ 6.058378, 52.931549 ], [ 6.058378, 52.931531 ], [ 6.058348, 52.931531 ], [ 6.058348, 52.931513 ], [ 6.058318, 52.931513 ], [ 6.058288, 52.931513 ], [ 6.058288, 52.931495 ], [ 6.058258, 52.931495 ], [ 6.058228, 52.931495 ], [ 6.058228, 52.931477 ], [ 6.058198, 52.931477 ], [ 6.058168, 52.931477 ], [ 6.058168, 52.931459 ], [ 6.058138, 52.931459 ], [ 6.058108, 52.931459 ], [ 6.058108, 52.931441 ], [ 6.058078, 52.931441 ], [ 6.058078, 52.931423 ], [ 6.058048, 52.931423 ], [ 6.058018, 52.931423 ], [ 6.058018, 52.931405 ], [ 6.057988, 52.931405 ], [ 6.057958, 52.931405 ], [ 6.057958, 52.931387 ], [ 6.057928, 52.931387 ], [ 6.057928, 52.931369 ], [ 6.057898, 52.931369 ], [ 6.057868, 52.931369 ], [ 6.057868, 52.931351 ], [ 6.057838, 52.931351 ], [ 6.057808, 52.931351 ], [ 6.057808, 52.931333 ], [ 6.057778, 52.931333 ], [ 6.057748, 52.931333 ], [ 6.057748, 52.931315 ], [ 6.057718, 52.931315 ], [ 6.057688, 52.931315 ], [ 6.057658, 52.931315 ], [ 6.057628, 52.931315 ], [ 6.057628, 52.931333 ], [ 6.057598, 52.931333 ], [ 6.057568, 52.931333 ], [ 6.057568, 52.931351 ], [ 6.057538, 52.931351 ], [ 6.057538, 52.931333 ], [ 6.057508, 52.931333 ], [ 6.057508, 52.931315 ], [ 6.057478, 52.931315 ], [ 6.057448, 52.931315 ], [ 6.057448, 52.931297 ], [ 6.057418, 52.931297 ], [ 6.057418, 52.931279 ], [ 6.057388, 52.931279 ], [ 6.057358, 52.931279 ], [ 6.057358, 52.931261 ], [ 6.057328, 52.931261 ], [ 6.057298, 52.931261 ], [ 6.057298, 52.931243 ], [ 6.057268, 52.931243 ] ] ] } }");

			return feature;
		}
		private Feature Map(ManualPrescription prescription)
		{
			Feature feature = null;
			// @ToDo or not to do? use field boundary?
			feature = JsonConvert.DeserializeObject<Feature>("{ \"type\": \"Feature\", \"properties\": {}, \"geometry\": { \"type\": \"Polygon\", \"coordinates\": [ [ [ 6.057268, 52.931243 ], [ 6.057268, 52.931225 ], [ 6.057238, 52.931225 ], [ 6.057208, 52.931225 ], [ 6.057208, 52.931243 ], [ 6.057208, 52.931261 ], [ 6.057178, 52.931261 ], [ 6.057178, 52.931279 ], [ 6.057148, 52.931279 ], [ 6.057148, 52.931297 ], [ 6.057148, 52.931315 ], [ 6.057118, 52.931315 ], [ 6.057118, 52.931333 ], [ 6.057088, 52.931333 ], [ 6.057088, 52.931351 ], [ 6.057088, 52.931369 ], [ 6.057058, 52.931369 ], [ 6.057058, 52.931387 ], [ 6.057058, 52.931405 ], [ 6.057028, 52.931405 ], [ 6.057028, 52.931423 ], [ 6.056998, 52.931423 ], [ 6.056998, 52.931441 ], [ 6.056998, 52.931459 ], [ 6.056968, 52.931459 ], [ 6.056968, 52.931477 ], [ 6.056938, 52.931477 ], [ 6.056938, 52.931495 ], [ 6.056938, 52.931513 ], [ 6.056908, 52.931513 ], [ 6.056908, 52.931531 ], [ 6.056908, 52.931549 ], [ 6.056878, 52.931549 ], [ 6.056878, 52.931567 ], [ 6.056848, 52.931567 ], [ 6.056848, 52.931585 ], [ 6.056848, 52.931603 ], [ 6.056818, 52.931603 ], [ 6.056818, 52.931621 ], [ 6.056788, 52.931621 ], [ 6.056788, 52.931639 ], [ 6.056788, 52.931657 ], [ 6.056758, 52.931657 ], [ 6.056758, 52.931675 ], [ 6.056758, 52.931693 ], [ 6.056728, 52.931693 ], [ 6.056728, 52.931711 ], [ 6.056698, 52.931711 ], [ 6.056698, 52.931729 ], [ 6.056698, 52.931747 ], [ 6.056668, 52.931747 ], [ 6.056668, 52.931765 ], [ 6.056638, 52.931765 ], [ 6.056638, 52.931783 ], [ 6.056638, 52.931801 ], [ 6.056608, 52.931801 ], [ 6.056608, 52.931819 ], [ 6.056608, 52.931837 ], [ 6.056578, 52.931837 ], [ 6.056578, 52.931855 ], [ 6.056548, 52.931855 ], [ 6.056548, 52.931873 ], [ 6.056548, 52.931891 ], [ 6.056518, 52.931891 ], [ 6.056518, 52.931909 ], [ 6.056488, 52.931909 ], [ 6.056488, 52.931927 ], [ 6.056488, 52.931945 ], [ 6.056458, 52.931945 ], [ 6.056458, 52.931963 ], [ 6.056458, 52.931981 ], [ 6.056428, 52.931981 ], [ 6.056428, 52.931999 ], [ 6.056398, 52.931999 ], [ 6.056398, 52.932017 ], [ 6.056398, 52.932035 ], [ 6.056368, 52.932035 ], [ 6.056368, 52.932053 ], [ 6.056338, 52.932053 ], [ 6.056338, 52.932071 ], [ 6.056338, 52.932089 ], [ 6.056308, 52.932089 ], [ 6.056308, 52.932107 ], [ 6.056308, 52.932125 ], [ 6.056278, 52.932125 ], [ 6.056278, 52.932143 ], [ 6.056248, 52.932143 ], [ 6.056248, 52.932161 ], [ 6.056248, 52.932179 ], [ 6.056218, 52.932179 ], [ 6.056218, 52.932197 ], [ 6.056188, 52.932197 ], [ 6.056188, 52.932215 ], [ 6.056188, 52.932233 ], [ 6.056158, 52.932233 ], [ 6.056158, 52.932251 ], [ 6.056188, 52.932251 ], [ 6.056218, 52.932251 ], [ 6.056218, 52.932269 ], [ 6.056248, 52.932269 ], [ 6.056248, 52.932287 ], [ 6.056278, 52.932287 ], [ 6.056308, 52.932287 ], [ 6.056308, 52.932305 ], [ 6.056338, 52.932305 ], [ 6.056338, 52.932323 ], [ 6.056368, 52.932323 ], [ 6.056398, 52.932323 ], [ 6.056398, 52.932341 ], [ 6.056428, 52.932341 ], [ 6.056458, 52.932341 ], [ 6.056458, 52.932359 ], [ 6.056488, 52.932359 ], [ 6.056488, 52.932377 ], [ 6.056518, 52.932377 ], [ 6.056548, 52.932377 ], [ 6.056548, 52.932395 ], [ 6.056578, 52.932395 ], [ 6.056608, 52.932395 ], [ 6.056608, 52.932413 ], [ 6.056638, 52.932413 ], [ 6.056638, 52.932395 ], [ 6.056638, 52.932377 ], [ 6.056668, 52.932377 ], [ 6.056668, 52.932359 ], [ 6.056698, 52.932359 ], [ 6.056698, 52.932341 ], [ 6.056698, 52.932323 ], [ 6.056728, 52.932323 ], [ 6.056728, 52.932341 ], [ 6.056758, 52.932341 ], [ 6.056788, 52.932341 ], [ 6.056788, 52.932359 ], [ 6.056818, 52.932359 ], [ 6.056848, 52.932359 ], [ 6.056848, 52.932377 ], [ 6.056878, 52.932377 ], [ 6.056878, 52.932395 ], [ 6.056908, 52.932395 ], [ 6.056938, 52.932395 ], [ 6.056938, 52.932413 ], [ 6.056968, 52.932413 ], [ 6.056998, 52.932413 ], [ 6.056998, 52.932431 ], [ 6.057028, 52.932431 ], [ 6.057028, 52.932449 ], [ 6.057058, 52.932449 ], [ 6.057088, 52.932449 ], [ 6.057088, 52.932467 ], [ 6.057118, 52.932467 ], [ 6.057118, 52.932485 ], [ 6.057148, 52.932485 ], [ 6.057178, 52.932485 ], [ 6.057178, 52.932503 ], [ 6.057208, 52.932503 ], [ 6.057238, 52.932503 ], [ 6.057238, 52.932521 ], [ 6.057268, 52.932521 ], [ 6.057268, 52.932539 ], [ 6.057298, 52.932539 ], [ 6.057328, 52.932539 ], [ 6.057328, 52.932557 ], [ 6.057358, 52.932557 ], [ 6.057358, 52.932539 ], [ 6.057388, 52.932539 ], [ 6.057388, 52.932521 ], [ 6.057418, 52.932521 ], [ 6.057418, 52.932503 ], [ 6.057418, 52.932485 ], [ 6.057448, 52.932485 ], [ 6.057448, 52.932467 ], [ 6.057448, 52.932449 ], [ 6.057478, 52.932449 ], [ 6.057478, 52.932431 ], [ 6.057508, 52.932431 ], [ 6.057508, 52.932413 ], [ 6.057508, 52.932395 ], [ 6.057538, 52.932395 ], [ 6.057538, 52.932377 ], [ 6.057568, 52.932377 ], [ 6.057568, 52.932359 ], [ 6.057568, 52.932341 ], [ 6.057598, 52.932341 ], [ 6.057598, 52.932323 ], [ 6.057598, 52.932305 ], [ 6.057628, 52.932305 ], [ 6.057628, 52.932287 ], [ 6.057658, 52.932287 ], [ 6.057658, 52.932269 ], [ 6.057658, 52.932251 ], [ 6.057688, 52.932251 ], [ 6.057688, 52.932233 ], [ 6.057688, 52.932215 ], [ 6.057718, 52.932215 ], [ 6.057718, 52.932197 ], [ 6.057748, 52.932197 ], [ 6.057748, 52.932179 ], [ 6.057748, 52.932161 ], [ 6.057778, 52.932161 ], [ 6.057778, 52.932143 ], [ 6.057808, 52.932143 ], [ 6.057808, 52.932125 ], [ 6.057808, 52.932107 ], [ 6.057838, 52.932107 ], [ 6.057838, 52.932089 ], [ 6.057868, 52.932089 ], [ 6.057868, 52.932071 ], [ 6.057868, 52.932053 ], [ 6.057898, 52.932053 ], [ 6.057898, 52.932035 ], [ 6.057928, 52.932035 ], [ 6.057928, 52.932017 ], [ 6.057928, 52.931999 ], [ 6.057958, 52.931999 ], [ 6.057958, 52.931981 ], [ 6.057958, 52.931963 ], [ 6.057988, 52.931963 ], [ 6.057988, 52.931945 ], [ 6.058018, 52.931945 ], [ 6.058018, 52.931927 ], [ 6.058018, 52.931909 ], [ 6.058048, 52.931909 ], [ 6.058048, 52.931891 ], [ 6.058078, 52.931891 ], [ 6.058078, 52.931873 ], [ 6.058078, 52.931855 ], [ 6.058108, 52.931855 ], [ 6.058108, 52.931837 ], [ 6.058138, 52.931837 ], [ 6.058138, 52.931819 ], [ 6.058138, 52.931801 ], [ 6.058168, 52.931801 ], [ 6.058168, 52.931783 ], [ 6.058168, 52.931765 ], [ 6.058198, 52.931765 ], [ 6.058198, 52.931747 ], [ 6.058228, 52.931747 ], [ 6.058228, 52.931729 ], [ 6.058228, 52.931711 ], [ 6.058258, 52.931711 ], [ 6.058258, 52.931693 ], [ 6.058258, 52.931675 ], [ 6.058288, 52.931675 ], [ 6.058288, 52.931657 ], [ 6.058318, 52.931657 ], [ 6.058318, 52.931639 ], [ 6.058318, 52.931621 ], [ 6.058318, 52.931603 ], [ 6.058348, 52.931603 ], [ 6.058348, 52.931585 ], [ 6.058378, 52.931585 ], [ 6.058378, 52.931567 ], [ 6.058378, 52.931549 ], [ 6.058378, 52.931531 ], [ 6.058348, 52.931531 ], [ 6.058348, 52.931513 ], [ 6.058318, 52.931513 ], [ 6.058288, 52.931513 ], [ 6.058288, 52.931495 ], [ 6.058258, 52.931495 ], [ 6.058228, 52.931495 ], [ 6.058228, 52.931477 ], [ 6.058198, 52.931477 ], [ 6.058168, 52.931477 ], [ 6.058168, 52.931459 ], [ 6.058138, 52.931459 ], [ 6.058108, 52.931459 ], [ 6.058108, 52.931441 ], [ 6.058078, 52.931441 ], [ 6.058078, 52.931423 ], [ 6.058048, 52.931423 ], [ 6.058018, 52.931423 ], [ 6.058018, 52.931405 ], [ 6.057988, 52.931405 ], [ 6.057958, 52.931405 ], [ 6.057958, 52.931387 ], [ 6.057928, 52.931387 ], [ 6.057928, 52.931369 ], [ 6.057898, 52.931369 ], [ 6.057868, 52.931369 ], [ 6.057868, 52.931351 ], [ 6.057838, 52.931351 ], [ 6.057808, 52.931351 ], [ 6.057808, 52.931333 ], [ 6.057778, 52.931333 ], [ 6.057748, 52.931333 ], [ 6.057748, 52.931315 ], [ 6.057718, 52.931315 ], [ 6.057688, 52.931315 ], [ 6.057658, 52.931315 ], [ 6.057628, 52.931315 ], [ 6.057628, 52.931333 ], [ 6.057598, 52.931333 ], [ 6.057568, 52.931333 ], [ 6.057568, 52.931351 ], [ 6.057538, 52.931351 ], [ 6.057538, 52.931333 ], [ 6.057508, 52.931333 ], [ 6.057508, 52.931315 ], [ 6.057478, 52.931315 ], [ 6.057448, 52.931315 ], [ 6.057448, 52.931297 ], [ 6.057418, 52.931297 ], [ 6.057418, 52.931279 ], [ 6.057388, 52.931279 ], [ 6.057358, 52.931279 ], [ 6.057358, 52.931261 ], [ 6.057328, 52.931261 ], [ 6.057298, 52.931261 ], [ 6.057298, 52.931243 ], [ 6.057268, 52.931243 ] ] ] } }");
			feature.Properties.Add("ApplicationStrategy", prescription.ApplicationStrategy);    // Enum: RatePerArea, RatePerTank, TotalProduct
			feature.Properties.Add("ProductUses", prescription.ProductUses);					// ProductId, Rate, AppliedArea , ProductTotal
			feature.Properties.Add("TankAmount", prescription.TankAmount);
			feature.Properties.Add("TotalArea", prescription.TotalArea);
			feature.Properties.Add("TotalTanks", prescription.TotalTanks);

			return feature;
		}
		private Feature Map(VectorPrescription prescription)
		{
			Feature feature = null;
			GeoJSON.Net.Geometry.Polygon polygon = null;
			Dictionary<string, object> properties = new Dictionary<string, object>();
			if (prescription.BoundingBox != null)
			{ 
				polygon = PolygonMapper.MapBoundingBox(prescription.BoundingBox);
				properties.Add("BoundingBox", prescription.BoundingBox);            // MinX, MinY, MaxX, MaxY
			}
			// ShapeType Enum: Point, MultiPoint, LineString, MultiLineString, LinearRing, Polygon, MultiPolygon
			properties.Add("RxShapeLookups", prescription.RxShapeLookups);      // Shape (MultiPolygon), Rates (1)
			properties.Add("RxProductLookups", prescription.RxProductLookups);  // Id, ProductId, Representation, UnitOfMeasure
																				// SpatialPrescription 
			if (prescription.OutOfFieldRate != null)
				properties.Add("OutOfFieldRate", prescription.OutOfFieldRate.Value.Value);
			if (prescription.LossOfGpsRate != null)
				properties.Add("LossOfGpsRate", prescription.LossOfGpsRate.Value.Value);

			feature = new Feature(polygon, properties);
			
			return feature;
		}
		private List<Feature> MapMultiple(VectorPrescription prescription)
		{
			List<Feature> features = new List<Feature>();

			foreach (var shaperate in prescription.RxShapeLookups)
            {
				//prescription.RxProductLookups
				int index = 0;
				if (shaperate.Shape != null && shaperate.Shape.Polygons.Count > 0)
				{
					foreach (var adaptPolygon in shaperate.Shape.Polygons)
					{
						Dictionary<string, object> properties = new Dictionary<string, object>();
						properties.Add("product", shaperate.Rates[index].RxProductLookupId);
						properties.Add("rate", shaperate.Rates[index++].Rate);
						
						features.Add(new Feature(PolygonMapper.MapPolygon(adaptPolygon), properties));
					}
				}
			}

			return features;
		}

		private Feature Map(RasterGridPrescription prescription, int gridType)
		{
			Feature feature = null;
			GeoJSON.Net.Geometry.IGeometryObject geometry = null;
			Dictionary<string, object> properties = new Dictionary<string, object>();
			if (prescription.BoundingBox != null)
			{
				geometry = PolygonMapper.MapBoundingBox(prescription.BoundingBox);
				properties.Add("BoundingBox", prescription.BoundingBox);
			}
			else
			{
				geometry = PointMapper.MapPoint2Point(prescription.Origin);
				properties.Add("Origin", prescription.Origin);
			}
			properties.Add("gridType", gridType);
			properties.Add("RowCount", prescription.RowCount);
			properties.Add("ColumnCount", prescription.ColumnCount);
			properties.Add("CellWidth", prescription.CellWidth.Value.Value);
			properties.Add("CellHeight", prescription.CellHeight.Value.Value);
			properties.Add("Rates", prescription.Rates);
			// SpatialPrescription 
			if (prescription.OutOfFieldRate != null)
				properties.Add("OutOfFieldRate", prescription.OutOfFieldRate.Value.Value);
			if (prescription.LossOfGpsRate != null)
				properties.Add("LossOfGpsRate", prescription.LossOfGpsRate.Value.Value);

			feature = new Feature(geometry, properties);

			return feature;
		}
		private List<Feature> MapMultiple(RasterGridPrescription prescription, int gridType)
		{
			List<Feature> features = new List<Feature>();
			
			// Based on Open.Topology.TestRunner.Functions/CreateShapeFunctions.Grid
			var grid = new List<BoundingBox>();
			int nCellsOnSideX = (int)prescription.ColumnCount;
			int nCellsOnSideY = (int)prescription.RowCount;
			double cellSizeX = prescription.CellWidth.Value.Value;
			double cellSizeY = prescription.CellHeight.Value.Value;

			BoundingBox env;
			if (prescription.BoundingBox != null)
			{
				env = prescription.BoundingBox;
			}
			else
			{
				env = new BoundingBox();
				env.MinY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), prescription.Origin.Y));
				env.MinX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), prescription.Origin.X));
				//env.MaxY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), prescription.Origin.Y + nCellsOnSideY * cellSizeY));
				//env.MaxX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), prescription.Origin.X + nCellsOnSideX * cellSizeX));
			}

			for (int j = 0; j < nCellsOnSideY; j++)
			{
				for (int i = 0; i < nCellsOnSideX; i++)
				{
					double x1 = env.MinX.Value.Value + i * cellSizeX;
					double y1 = env.MinY.Value.Value + j * cellSizeY;
					double x2 = env.MinX.Value.Value + (i + 1) * cellSizeX;
					double y2 = env.MinY.Value.Value + (j + 1) * cellSizeY;

					var bbox = new BoundingBox();
					bbox.MinY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), y1));
					bbox.MinX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), x1));
					bbox.MaxY = new NumericRepresentationValue(RepresentationInstanceList.vrLatitude.ToModelRepresentation(),  new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), y2));
					bbox.MaxX = new NumericRepresentationValue(RepresentationInstanceList.vrLongitude.ToModelRepresentation(), new NumericValue(UnitSystemManager.GetUnitOfMeasure("arcdeg"), x2));
					grid.Add(bbox);
				}
			}

			int index = 0;
			double outOfFieldRate = -1.0;
			if (prescription.OutOfFieldRate != null)
				outOfFieldRate = prescription.OutOfFieldRate.Value.Value;

			Console.WriteLine($"PrescriptionMapper outOfFieldRate: {outOfFieldRate}, grids {grid.Count}, env {env}");

			foreach (var bbox in grid)
			{
				// skip outOfField grids
				if (prescription.Rates[index].RxRates[0].Rate != outOfFieldRate)
                {
					GeoJSON.Net.Geometry.IGeometryObject geometry = PolygonMapper.MapBoundingBox(bbox);
					Dictionary<string, object> properties = new Dictionary<string, object>();
					//prescription.RxProductLookups
					properties.Add("product", prescription.Rates[index].RxRates[0].RxProductLookupId);
					properties.Add("rate", prescription.Rates[index].RxRates[0].Rate);

					features.Add(new Feature(geometry, properties));
				}
				index++;
			}

			return features;
		}

		public Feature MapAsSingleFeature(Prescription adaptPrescription, int gridType)
		{
			Feature prescriptionFeature = null;

			// Prescription, types
			if (adaptPrescription is RasterGridPrescription)
			{
				prescriptionFeature = Map(adaptPrescription as RasterGridPrescription, gridType);
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
				prescriptionFeature.Properties.Add("FieldId", adaptPrescription.FieldId);
					
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
		public List<Feature> MapAsMultipleFeatures(Prescription adaptPrescription, int gridType)
		{
			Feature prescriptionFeature = null;
			List<Feature> prescriptionFeatures = new List<Feature>();

			// Prescription, types
			if (adaptPrescription is RasterGridPrescription)
			{
				prescriptionFeatures = MapMultiple(adaptPrescription as RasterGridPrescription, gridType);
			}
			else if (adaptPrescription is VectorPrescription)
			{
				prescriptionFeatures = MapMultiple(adaptPrescription as VectorPrescription);
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
				prescriptionFeature.Properties.Add("FieldId", adaptPrescription.FieldId);

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