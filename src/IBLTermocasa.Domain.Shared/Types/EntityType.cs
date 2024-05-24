using System;
using System.Collections.Generic;
using System.Text;

namespace IBLTermocasa.Types
{
    public enum EntityType
    {
        [Path("/rfq")]
        RFQ,
        [Path("/quote")]
        QUOTE,
        [Path("/working-sheet")]
        WORKINGSHEET,
        [Path("/technical-sheet")]
        TECHNICALSHEET,
        [Path("/bom")]
        BOM,
        [Path("/work-order")]
        WORKORDER
    }

    public class PathAttribute : Attribute
    {
        public string Path { get; private set; }

        public PathAttribute(string path)
        {
            Path = path;
        }
    }

    public static class EntityTypeExtensions
    {
        public static string GetPath(this EntityType entityType)
        {
            var type = entityType.GetType();
            var name = Enum.GetName(type, entityType);
            var field = type.GetField(name);
            var attribute = Attribute.GetCustomAttribute(field, typeof(PathAttribute)) as PathAttribute;
            return attribute?.Path;
        }
    }
}
