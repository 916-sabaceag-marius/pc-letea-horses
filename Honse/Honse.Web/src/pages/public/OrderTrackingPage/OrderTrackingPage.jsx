import React, { useEffect, useState } from "react";
import { getOrderDetailsAPI, cancelOrderAPI } from "../../../services/orderService";
import { useNavigate, useParams } from "react-router";

export default function OrderTrackingPage() {
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isCancelModalOpen, setIsCancelModalOpen] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);

  const restaurantId = '550e8400-e29b-41d4-a716-446655440000';
  const orderId = 'order-2';
  const ORDER_FLOW = [
    { status: 0, description: "Order Confirmed", icon: "receipt_long" },
    { status: 1, description: "Your order is being prepared", icon: "restaurant_menu" },
    { status: 2, description: "Order picked up for delivery", icon: "local_shipping" },
    { status: 3, description: "Order Delivered", icon: "check_circle" },
  ];
  function getEventTime(statusCode, order) {
    const orderDate = new Date(order.orderDate + "T" + order.orderTime);

    switch (statusCode) {
      case 0: // Confirmed
        return orderDate.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
      case 1: // Preparing
        return new Date(orderDate.getTime() + 3 * 60000).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }); // +3 min
      case 2: // Out for delivery
        return new Date(orderDate.getTime() + 15 * 60000).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }); // +15 min
      case 3: // Delivered
        return new Date(orderDate.getTime() + 30 * 60000).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }); // +30 min
      default:
        return "";
    }
  }

  useEffect(() => {
    async function loadOrder() {
      const res = await getOrderDetailsAPI(restaurantId, orderId);

      if (res.succeeded) {
        setOrder(res.data);
      } else {
        console.error(res.errorMessage);
      }

      setLoading(false);
    }

    loadOrder();
  }, [restaurantId, orderId]);

const navigate = useNavigate();

async function handleCancelOrder() {
  setIsCancelling(true);
  try {
    const res = await cancelOrderAPI(order.restaurantId, order.id);
    if (res.succeeded) {

      navigate("/public/restaurants");
    } else {
      console.error(res.errorMessage);
      setIsCancelling(false);
    }
  } catch (err) {
    console.error(err);
    setIsCancelling(false);
  }
}


  if (!order) return <p className="p-10 text-lg text-red-500">Order not found.</p>;

  const items = order.items ?? [];
  const client = order.customer; // {name, email, address}
  const orderNo = order.orderNumber;
  const subtotal = items.reduce((sum, item) => sum + item.total, 0);
  const deliveryFee = 5;
  const total = subtotal + deliveryFee;
  const currentStatus = order.status;
  const visibleHistory = ORDER_FLOW.filter(event => event.status <= currentStatus);


  const deliveryTime = new Date(order.deliveryTime);
  const now = new Date();
  const minutesRemaining = Math.max(
    0,
    Math.round((deliveryTime - now) / 60000)
  );

  // Progress bar percentage
  const statusStages = ["confirmed", "preparing", "out for delivery", "delivered"];
  const progressIndexMap = {
    0: 0, // New
    1: 1, // Preparing
    2: 2, // Out for Delivery
    3: 3, // Delivered
    [-1]: 0 // Cancelled
  };

  const statusIndex = progressIndexMap[order.status] ?? 0;
  const progressPercent = ((statusIndex + 1) / 4) * 100;
  const statusLabel = statusStages[statusIndex];

  return (
    <div className="max-w-7xl mx-auto">

      {/* Heading */}
      <div className="pt-6 pb-8">
        <h1 className="text-4xl font-extrabold">
          Your order is <span className="text-orange-500">{statusLabel}</span>!
        </h1>
        <p className="text-2xl text-gray-600">Order Number: {orderNo}</p>
      </div>

      <div className="w-full max-w-6xl grid grid-cols-1 lg:grid-cols-[2fr,1fr] gap-10">

        {/* LEFT COLUMN */}
        <div className="flex flex-col gap-8">

          {/* Order Items */}
          <div className="bg-white border border-[#e7d9cf] rounded-xl shadow-sm">
            <h2 className="text-lg font-bold p-4 border-b border-[#e7d9cf]">
              Order Items ({items.length})
            </h2>

            <ul className="divide-y divide-[#e7d9cf]">
              {items.map((item, i) => (
                <li key={i} className="p-4 flex gap-4">
                  <img
                    src={item.imgUrl || "/placeholder-food.jpg"}
                    alt={item.name}
                    className="w-16 h-16 rounded-md object-cover"
                  />
                  <div className="flex-1">
                    <p className="font-semibold">{item.name}</p>
                  </div>
                  <div className="text-right">
                    <p className="font-semibold">
                      {item.quantity} × ${item.price.toFixed(2)}
                    </p>
                    <p className="text-sm">${item.total.toFixed(2)}</p>
                  </div>
                </li>
              ))}
            </ul>

            {/* Totals */}
            <div className="border-t border-[#e7d9cf] p-4 text-sm space-y-2">
              <div className="flex justify-between"><span>Subtotal</span><span>${subtotal.toFixed(2)}</span></div>
              <div className="flex justify-between"><span>Delivery Fee</span><span>${deliveryFee.toFixed(2)}</span></div>
              <div className="flex justify-between font-bold text-base"><span>Total</span><span>${total.toFixed(2)}</span></div>
            </div>
          </div>

          {/* Progress Bar */}
          <div>
            <p className="text-lg font-bold">{statusLabel}</p>

            <div className="w-full bg-blue-100 rounded-lg h-3 mt-2">
              <div
                className="h-3 bg-blue-500 rounded-lg transition-all"
                style={{ width: `${progressPercent}%` }}
              ></div>
            </div>

            <div className="grid grid-cols-4 text-sm text-gray-600 mt-2">
              {statusStages.map((stage, i) => (
                <div
                  key={i}
                  className={`text-center font-medium ${statusIndex >= i ? "text-black" : ""}`}
                >
                  {stage}
                </div>
              ))}
            </div>
          </div>
          {order.status !== -1 && order.status !== 3 && (
            <button
              onClick={() => setIsCancelModalOpen(true)}
               className="mt-4 inline-flex px-3 py-1.5 bg-red-500 hover:bg-red-600 text-white font-bold text-sm rounded-lg mx-auto"
            >
              Cancel Order
            </button>
          )}

        </div>

        {/* RIGHT COLUMN */}
        <div className="flex flex-col gap-8">

          {/* ETA */}
          <div className="bg-white p-6 rounded-xl border border-[#e7d9cf] shadow-sm">
            <h2 className="text-lg font-bold mb-1">Estimated Delivery</h2>
            <p className="text-4xl font-black text-[#3b82f6]">
              {deliveryTime.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
            </p>
            <p className="text-gray-600 mt-1">
              Arriving in {minutesRemaining} minutes
            </p>
          </div>

          {/* Order History */}
          <div className="bg-white rounded-xl p-6 shadow-sm border border-gray-200">
            <h2 className="text-lg font-bold mb-4">Order History</h2>
            <ul className="space-y-4">
              {visibleHistory.map((event, index) => (
                <li key={index} className="flex items-start gap-4">
                  <div className="w-6 h-6 flex items-center justify-center rounded-full bg-orange-500 text-white mt-1">
                    <span className="material-symbols-outlined text-base">{event.icon}</span>
                  </div>
                  <div className="flex-1">
                    <p className="font-medium text-gray-900">{event.description}</p>
                    {/* For now, show approximate time based on order data */}
                    <p className="text-sm text-gray-500">
                      {getEventTime(event.status, order)}
                    </p>
                  </div>
                </li>
              ))}
            </ul>
          </div>

          {/* Customer Details */}
          <div className="rounded-xl border border-gray-200/50 bg-white shadow-sm">
            <h2 className="text-[#1b130d] text-lg font-bold p-4 border-b border-gray-200/50">
              Customer Details
            </h2>
            <div className="p-4 space-y-4">
              <div className="flex items-center gap-4">
                <span className="material-symbols-outlined text-gray-500">person</span>
                <div>
                  <p className="text-sm text-[#9a6c4c]">Name</p>
                  <p className="font-medium text-[#1b130d]">{client.name}</p>
                </div>
              </div>

              <div className="flex items-center gap-4">
                <span className="material-symbols-outlined text-gray-500">mail</span>
                <div>
                  <p className="text-sm text-[#9a6c4c]">Email</p>
                  <p className="font-medium text-[#1b130d]">{client.email}</p>
                </div>
              </div>

              <div className="flex items-start gap-4">
                <span className="material-symbols-outlined text-gray-500 mt-0.5">home</span>
                <div>
                  <p className="text-sm text-[#9a6c4c]">Delivery Address</p>
                  <p className="font-medium text-[#1b130d]">{client.address}</p>
                </div>
              </div>
            </div>
          </div>

        </div>

      </div>

      {isCancelModalOpen && (
  <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
    <div className="bg-white rounded-xl p-6 w-96 shadow-lg">
      <h2 className="text-xl font-bold mb-4">Confirm Cancellation</h2>
      <p className="mb-6">
        Are you sure you want to cancel your order? You will not receive a refund.
      </p>
      <div className="flex justify-end gap-4">
        <button
          onClick={() => setIsCancelModalOpen(false)}
          className="px-4 py-2 rounded-lg border border-gray-300"
        >
          Close
        </button>
        <button
          onClick={handleCancelOrder}
          className="px-4 py-2 rounded-lg bg-red-500 text-white hover:bg-red-600"
          disabled={isCancelling}
        >
          {isCancelling ? "Cancelling..." : "Confirm"}
        </button>
      </div>
    </div>
  </div>
)}

    </div>
  );
}
