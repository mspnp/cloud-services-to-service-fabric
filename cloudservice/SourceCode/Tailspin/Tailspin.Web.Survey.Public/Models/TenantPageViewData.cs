namespace Tailspin.Web.Survey.Public.Models
{
    public class TenantPageViewData<T> : TenantMasterPageViewData
    {
        private readonly T contentModel;

        public TenantPageViewData(T contentModel)
        {
            this.contentModel = contentModel;
        }

        public T ContentModel
        {
            get
            {
                return this.contentModel;
            }
        }
    }
}