using System;

namespace LegacyShop.Model
{
    /// <summary>
    /// Общий предок всех сущностей магазина. Заложен «на вырост»: когда-нибудь
    /// понадобится аудит (кто и когда менял), версионирование записей и единая
    /// валидация — тогда всё уже будет на месте.
    /// </summary>
    public abstract class ShopEntity
    {
        public string Id;
        public DateTime CreatedAt;
        public DateTime? UpdatedAt;
        public string CreatedBy;
        public string UpdatedBy;
        public int Version;
        public bool IsDeleted;

        public virtual string Describe()
        {
            return GetType().Name + " #" + Id;
        }

        public virtual bool Validate()
        {
            return true;
        }

        public virtual ShopEntity CloneForAudit()
        {
            return this;
        }
    }
}
