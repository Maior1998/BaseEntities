using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseEntities
{
    /// <summary>
    /// Представляет собой справочник, т.е. объект, у которого есть название и, возможно, описание.
    /// </summary>
    public interface IBaseLookup : IBaseEntity
    {
        /// <summary>
        /// Название данной записи справочника.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Возможное описание данной записи справочника.
        /// </summary>
        public string? Description { get; set; }
    }
}
