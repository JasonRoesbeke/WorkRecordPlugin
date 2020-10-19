using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfAppImportGPX.Resources;

namespace WpfAppImportGPX
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum ComponentType
    {
        [LocalizedDescription("Do Not Include", typeof(EnumResources))] ExceptionValue,
        [LocalizedDescription("Field Boundary", typeof(EnumResources))] FieldBoundary,
        [LocalizedDescription("Driven Headland", typeof(EnumResources))] DrivenHeadland,
        [LocalizedDescription("AB Line", typeof(EnumResources))] ABLine,
        [LocalizedDescription("AB Curve", typeof(EnumResources))] ABCurve
    }

}
