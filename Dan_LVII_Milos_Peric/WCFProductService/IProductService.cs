using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFProductService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IProductService
    {
        [OperationContract]
        List<Product> GetAllProducts();

        [OperationContract]
        void CreateReceipt(List<Product> list);

        [OperationContract]
        void ModifyProductStock(int id, int amount);

        [OperationContract]
        void AddNewProduct(Product product);

        [OperationContract]
        void ModifyProductPrice(int id, double price);
    }
}
