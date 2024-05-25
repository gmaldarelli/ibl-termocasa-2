using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace IBLTermocasa.Domain;

public class CustomGuidSerializationProvider : IBsonSerializationProvider
{
        
    public IBsonSerializer GetSerializer(Type type)
    {
        if (type == typeof(Guid)) return new CustomGuidSerializer(GuidRepresentation.Standard);

        return null; // falls back to Mongo defaults
    }
}