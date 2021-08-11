using System;
using System.ComponentModel.DataAnnotations;

namespace BaseEntities
{
    /// <summary>
    /// Представляет собой базовую сущность системы. 
    /// Содержит поля, которые по хорошему должны быть у всех объектов системы.
    /// </summary>
    public interface IBaseEntity
    {
        [Key] public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
