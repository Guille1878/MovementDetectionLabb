//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IndoorPlaceInformationAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class Entrance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Entrance()
        {
            this.SensorBoard = new HashSet<SensorBoard>();
        }
    
        public System.Guid EntranceId { get; set; }
        public System.Guid PlaceId { get; set; }
        public string Name { get; set; }
    
        public virtual Place Place { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SensorBoard> SensorBoard { get; set; }
    }
}
