//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StoryTree.Storage.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventTreeEntity
    {
        public long EventTreeId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public Nullable<long> Color { get; set; }
        public Nullable<long> MainTreeEventId { get; set; }
        public long ProjectId { get; set; }
        public long Order { get; set; }
    
        public virtual TreeEventEntity TreeEventEntity { get; set; }
        public virtual ProjectEntity ProjectEntity { get; set; }
    }
}
