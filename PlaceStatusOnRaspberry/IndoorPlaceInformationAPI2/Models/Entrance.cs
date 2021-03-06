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

    public partial class Entrance
    {
        /// <summary>
        /// Initializes a new instance of the Entrance class.
        /// </summary>
        public Entrance() { }

        /// <summary>
        /// Initializes a new instance of the Entrance class.
        /// </summary>
        public Entrance(Guid? entranceId = default(Guid?), Guid? placeId = default(Guid?), string name = default(string), Place place = default(Place), IList<SensorBoard> sensorBoard = default(IList<SensorBoard>))
        {
            EntranceId = entranceId;
            PlaceId = placeId;
            Name = name;
            Place = place;
            SensorBoard = sensorBoard;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "EntranceId")]
        public Guid? EntranceId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PlaceId")]
        public Guid? PlaceId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Place")]
        public Place Place { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SensorBoard")]
        public IList<SensorBoard> SensorBoard { get; set; }

    }
}
