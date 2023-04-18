using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Entities
{
    public class BaseEntity : IAggregateRoot
    {
        private Guid _id;
        protected bool _isDeleted;
        protected bool _isNew;
        public BaseEntity() { }

        protected BaseEntity(Guid id, bool isDeleted)
        {
            this._id = id;
            this._isDeleted = isDeleted;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; }
        }
     
        public DateTime LastProcessedEventTime => DateTime.Now;

        public bool IsNew { get ; set ; }

        public void Delete()
        {
            this._isDeleted = true;
        }

        public void MakeNew()
        {
            this._isNew = true;
        }
    }
}
