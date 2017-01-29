using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace IndoorPlaceInformationAPI.Models
{
    /// <summary>
    /// Place type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaceType
    {
        /// <summary>
        /// Toilet for Ladies
        /// </summary>
        ToiletLadies = 1,
        /// <summary>
        /// Toilet for Gentlemen
        /// </summary>
        ToiletGentlemen = 2,
        /// <summary>
        /// Toilet for Handicap
        /// </summary>
        ToiletAccessible = 3,
        /// <summary>
        /// Toilet for Baby
        /// </summary>
        ToiletBabyLounge = 4,
        /// <summary>
        /// Toilet for all kindes
        /// </summary>
        ToiletMix = 5


    }
}