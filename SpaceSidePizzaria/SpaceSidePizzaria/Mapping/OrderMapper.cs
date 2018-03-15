using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceSidePizzaria.Models;
using SpaceSidePizzariaBLL.Models;
using SpaceSidePizzariaDAL.Models;

namespace SpaceSidePizzaria.Mapping
{
    public static class OrderMapper
    {
        public static OrderDO OrderPOtoOrderDO(OrderPO from)
        {
            OrderDO to = new OrderDO();

            to.OrderID = from.OrderID;
            to.UserID = from.UserID;
            to.DriverID = from.UserID;
            to.IsDelivery = from.IsDelivery;
            to.OrderDate = from.OrderDate;
            to.OrderFulfilledTime = from.OrderFulfilledTime;
            to.Status = from.Status;
            to.Paid = from.Paid;
            to.Total = from.Total;
            to.DriverFirstName = from.DriverFirstName;
            to.BuyerName = from.BuyerName;
            to.BuyerAddress = from.BuyerAddress;

            return to;
        }

        public static OrderPO OrderDOtoOrderPO(OrderDO from)
        {
            OrderPO to = new OrderPO();

            to.OrderID = from.OrderID;
            to.UserID = from.UserID;
            to.DriverID = from.UserID;
            to.IsDelivery = from.IsDelivery;
            to.OrderDate = from.OrderDate;
            to.OrderFulfilledTime = from.OrderFulfilledTime;
            to.Status = from.Status;
            to.Paid = from.Paid;
            to.Total = from.Total;
            to.DriverFirstName = from.DriverFirstName;
            to.BuyerName = from.BuyerName;
            to.BuyerAddress = from.BuyerAddress;

            return to;
        }

        public static OrderBO OrderDOtoOrderBO(OrderDO from)
        {
            OrderBO to = new OrderBO();

            to.OrderID = from.OrderID;
            to.UserID = from.UserID;
            to.DriverID = from.UserID;
            to.IsDelivery = from.IsDelivery;
            to.OrderDate = from.OrderDate;
            to.OrderFulfilledTime = from.OrderFulfilledTime;
            to.Status = from.Status;
            to.Paid = from.Paid;
            to.Total = from.Total;
            to.DriverFirstName = from.DriverFirstName;
            to.BuyerName = from.BuyerName;
            to.BuyerAddress = from.BuyerAddress;

            return to;
        }

        public static OrderBO OrderPOtoOrderBO(OrderPO from)
        {
            OrderBO to = new OrderBO();

            to.OrderID = from.OrderID;
            to.UserID = from.UserID;
            to.DriverID = from.UserID;
            to.IsDelivery = from.IsDelivery;
            to.OrderDate = from.OrderDate;
            to.OrderFulfilledTime = from.OrderFulfilledTime;
            to.Status = from.Status;
            to.Paid = from.Paid;
            to.Total = from.Total;
            to.DriverFirstName = from.DriverFirstName;
            to.BuyerName = from.BuyerName;
            to.BuyerAddress = from.BuyerAddress;

            return to;
        }

        public static List<OrderPO> OrderDOsToOrderPOs(List<OrderDO> orderDOs)
        {
            return orderDOs.Select(orderDO => OrderDOtoOrderPO(orderDO)).ToList();
        }

        public static List<OrderBO> OrderDOsToOrderBOs(List<OrderDO> orderDOs)
        {
            return orderDOs.Select(orderDO => OrderDOtoOrderBO(orderDO)).ToList();
        }

        public static List<OrderBO> OrderPOsToOrderBOs(List<OrderPO> orderPOs)
        {
            return orderPOs.Select(pizzaPO => OrderPOtoOrderBO(pizzaPO)).ToList();
        }
    }
}