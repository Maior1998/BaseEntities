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
        /// <summary>
        /// Номер данной записи в базе данных.
        /// </summary>
        [Key] public Guid Id { get; set; }
        /// <summary>
        /// Время создания данной записи.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Время последнего изменения данной записи.
        /// </summary>
        public DateTime ModifiedOn { get; set; }
    }
}
