namespace Tailspin.Web.Models
{
    public class PageViewData<T> : MasterPageViewData
    {
        private readonly T contentModel;

        public PageViewData(T contentModel)
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