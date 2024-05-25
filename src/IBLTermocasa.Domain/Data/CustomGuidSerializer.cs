using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace IBLTermocasa.Domain;

public class CustomGuidSerializer : GuidSerializer
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public CustomGuidSerializer(GuidRepresentation representation) : base(representation)
    {
    }

    public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();
        if (type == BsonType.String)
        {
            var guidString = context.Reader.ReadString();
            return Guid.Parse(guidString);
        }
        else
        {
            return base.Deserialize(context, args);
        }
    }
}
