﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace PlaceStatusOnRaspberry.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class Type
    {
        /// <summary>
        /// Initializes a new instance of the Type class.
        /// </summary>
        public Type() { }

        /// <summary>
        /// Initializes a new instance of the Type class.
        /// </summary>
        public Type(int? typeId = default(int?), string name = default(string), IList<Place> place = default(IList<Place>))
        {
            TypeId = typeId;
            Name = name;
            Place = place;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "TypeId")]
        public int? TypeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Place")]
        public IList<Place> Place { get; set; }

    }
}