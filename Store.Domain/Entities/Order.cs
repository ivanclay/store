using Store.Domain.Enums;
using Flunt.Validations;

namespace Store.Domain.Entities;
public class Order : Entity
{
    public Order(Customer customer, decimal deliveryFee, Discount discount)
    {
        AddNotifications(
            new Contract<Order>()
            .Requires()
            .IsNotNull(customer, "Customer", "Invalid customer.")
        );
        
        Customer = customer;
        Date = DateTime.Now;
        Number = Guid.NewGuid().ToString().Substring(0,8);
        DeliveryFee = deliveryFee;
        Discount = discount;
        Status = EOrderStatus.WaitingPayment;
        Items = new List<OrderItem>();
    }


    public Customer Customer { get; private set; }
    public DateTime Date { get; private set; }
    public string Number { get; private set; }
    public IList<OrderItem> Items { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public Discount Discount { get; private set; }
    public EOrderStatus Status { get; private set; }
    
        public void AddItem(Product product, int quantity)
        {
            var item = new OrderItem(product, quantity);
            if(item.IsValid)
                Items.Add(item);
        }

        public decimal Total()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Total();
            }

            total += DeliveryFee;
            total -= Discount != null ? Discount.Value() : 0;

            return total;
        }

        public void Pay(decimal amount)
        {
            if (amount == Total())
                this.Status = EOrderStatus.WaitingDelivery;
        }

        public void Cancel()
        {
            Status = EOrderStatus.Canceled;
        }
        

}