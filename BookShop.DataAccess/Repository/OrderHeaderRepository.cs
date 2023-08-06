using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
            var orderFromDB = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(orderFromDB != null)
            {
                orderFromDB.OrderStatus = orderStatus;
                if (!String.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDB.PaymentStatus = paymentStatus;
                }
            }
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
            var orderFromDB = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!String.IsNullOrEmpty(sessionId))
            {
                orderFromDB.SessionId = sessionId;
            }
			if (!String.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDB.PaymentIntentId = paymentIntentId;
                orderFromDB.PaymentDate = DateTime.Now;
			}
		}
	}
}
