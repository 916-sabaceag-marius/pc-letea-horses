import axios from "axios";

const RAW_BASE = process.env.REACT_APP_API_URL || "https://localhost:2000";
const BASE_URL = RAW_BASE.replace(/\/+$/, "");

export const api = axios.create({
    baseURL: BASE_URL,
    headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("token");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

export function failure(message) {
    return { succeeded: false, errorMessage: message || "Unexpected error." };
}

export function parseError(err, fallback) {
    const data = err?.response?.data;
    if (typeof data === "string") return data;
    if (data?.errorMessage) return data.errorMessage;
    if (data?.message) return data.message;
    return fallback;
}

function successData(data, extra = {}) {
    return { succeeded: true, data, ...extra };
}

// Order Status Enum Mapping (matches backend)
export const OrderStatus = {
    New: 0,
    Accepted: 1,
    Delivery: 2,
    Finished: 3,
    Cancelled: -1
};

export const getStatusLabel = (status) => {
    const statusMap = {
        0: "New",
        1: "Preparing",
        2: "Out for Delivery",
        3: "Delivered",
        [-1]: "Cancelled"
    };
    return statusMap[status] || "Unknown";
};

export const getStatusColor = (status) => {
    const colorMap = {
        0: "#3b82f6",      // New - Blue
        1: "#f59e0b",      // Preparing - Orange
        2: "#8b5cf6",      // Out for Delivery - Purple
        3: "#10b981",      // Delivered - Green
        [-1]: "#ef4444"    // Cancelled - Red
    };
    return colorMap[status] || "#6b7280";
};

export const getNextStatus = (currentStatus) => {
    const statusFlow = {
        [OrderStatus.New]: { next: OrderStatus.Accepted, label: "Accept" },
        [OrderStatus.Accepted]: { next: OrderStatus.Delivery, label: "Ready for Delivery" },
        [OrderStatus.Delivery]: { next: OrderStatus.Finished, label: "Mark as Delivered" },
        [OrderStatus.Finished]: null,
        [OrderStatus.Cancelled]: null
    };
    return statusFlow[currentStatus];
};

export async function getOrderDetailsAPI(restaurantId, orderId) {
    try {
        const order = MOCK_ORDERS.find(o => o.id === orderId && o.restaurantId === restaurantId);

        if (!order) {
            return failure("Order not found");
        }
        return successData(order);
    } catch (err) {
        return failure(parseError(err, "Failed to fetch order details"));
    }
}

export async function cancelOrderAPI(restaurantId, orderId) {
    try { 
        return successData();
    } catch (err) {
        return failure(parseError(err, "Failed to cancel order"));
    }
}



export const MOCK_RESTAURANT_ID = "550e8400-e29b-41d4-a716-446655440000";

const MOCK_ORDERS = [
    {
        id: "order-1",
        orderNumber: "#ORD-2023-12345",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Delivery,
        customerName: "John Doe",
        customer: {
            name: "John Doe",
            phone: "+1 (555) 987-6543",
            address: "456 Oak Street, New York, NY 10002"
        },
        items: [
            { id: "1", name: "Margherita Pizza", description: "Extra cheese", quantity: 1, price: 12.99 },
            { id: "2", name: "Coca Cola", description: "", quantity: 2, price: 2.50 }
        ],
        subtotal: 18.99,
        tax: 0.90,
        deliveryFee: 6.61,
        total: 25.50,
        orderDate: "12/8/2025",
        orderTime: "1:29:55 AM",
        timeAgo: "2 minutes ago"
    },
    {
        id: "order-2",
        orderNumber: "#ORD-2023-12346",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Delivery,
        customerName: "Jane Smith",
        customer: {
            name: "Jane Smith",
            email: "janeSmith@gmail.com",
            address: "123 Main St, New York, NY 10001"
        },
        items: [
            { id: "1", name: "Burger", description: "", quantity: 2, price: 15.00, total: 30, imgUrl: 'https://i.pinimg.com/736x/68/ef/90/68ef9032fae3d204d6f5bb72221d9b6e.jpg' },
             { id: "1", name: "Pasta", description: "", quantity: 1, price: 42.75, total: 42.75, imgUrl: 'https://i.pinimg.com/736x/68/ef/90/68ef9032fae3d204d6f5bb72221d9b6e.jpg',  }
        ],
        subtotal: 15.00,
        tax: 0.75,
        deliveryFee: 5.00,
        total: 15.00,
        deliveryTime: "2025-12-09T16:45:00",
         orderDate: "2025-12-09",
        orderTime: "14:48:55",
        timeAgo: "5 minutes ago"
    },
    {
        id: "order-3",
        orderNumber: "#ORD-2023-12347",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Cancelled,
        customerName: "Peter Jones",
        customer: {
            name: "Peter Jones",
            phone: "+1 (555) 234-5678",
            address: "789 Elm St, New York, NY 10003"
        },
        items: [
            { id: "1", name: "Pasta", description: "", quantity: 1, price: 42.75 }
        ],
        subtotal: 42.75,
        tax: 2.14,
        deliveryFee: 5.00,
        total: 42.75,
        orderDate: "12/8/2025",
        orderTime: "1:28:55 AM",
        timeAgo: "1 minute ago"
    },
    {
        id: "order-4",
        orderNumber: "#ORD-2023-12340",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Accepted,
        customerName: "Mary Jane",
        customer: {
            name: "Mary Jane",
            phone: "+1 (555) 345-6789",
            address: "321 Pine St, New York, NY 10004"
        },
        items: [
            { id: "1", name: "Sushi Platter", description: "", quantity: 1, price: 33.20 }
        ],
        subtotal: 33.20,
        tax: 1.66,
        deliveryFee: 5.00,
        total: 33.20,
        orderDate: "12/8/2025",
        orderTime: "1:14:55 AM",
        timeAgo: "15 minutes ago"
    },
    {
        id: "order-5",
        orderNumber: "#ORD-2023-12339",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Accepted,
        customerName: "Alex Ray",
        customer: {
            name: "Alex Ray",
            phone: "+1 (555) 456-7890",
            address: "654 Maple Ave, New York, NY 10005"
        },
        items: [
            { id: "1", name: "Tacos", description: "", quantity: 3, price: 6.66 }
        ],
        subtotal: 19.99,
        tax: 1.00,
        deliveryFee: 5.00,
        total: 19.99,
        orderDate: "12/8/2025",
        orderTime: "1:11:55 AM",
        timeAgo: "18 minutes ago"
    },
    {
        id: "order-6",
        orderNumber: "#ORD-2023-12335",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Delivery,
        customerName: "Sarah Connor",
        customer: {
            name: "Sarah Connor",
            phone: "+1 (555) 432-1098",
            address: "147 Birch Road, New York, NY 10007"
        },
        items: [
            { id: "1", name: "Steak", description: "Medium rare", quantity: 1, price: 55.00 }
        ],
        subtotal: 55.00,
        tax: 2.75,
        deliveryFee: 5.00,
        total: 55.00,
        orderDate: "12/8/2025",
        orderTime: "1:04:55 AM",
        timeAgo: "25 minutes ago"
    },
    {
        id: "order-7",
        orderNumber: "#ORD-2023-12330",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Delivery,
        customerName: "Mike Tyson",
        customer: {
            name: "Mike Tyson",
            phone: "+1 (555) 567-8901",
            address: "852 Cedar Blvd, New York, NY 10006"
        },
        items: [
            { id: "1", name: "Chicken Wings", description: "Extra spicy", quantity: 2, price: 44.05 }
        ],
        subtotal: 88.10,
        tax: 4.41,
        deliveryFee: 5.00,
        total: 88.10,
        orderDate: "12/8/2025",
        orderTime: "12:54:55 AM",
        timeAgo: "35 minutes ago"
    },
    {
        id: "order-8",
        orderNumber: "#ORD-2023-12325",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Finished,
        customerName: "Laura Croft",
        customer: {
            name: "Laura Croft",
            phone: "+1 (555) 678-9012",
            address: "963 Spruce Lane, New York, NY 10008"
        },
        items: [
            { id: "1", name: "Salad", description: "", quantity: 1, price: 21.50 }
        ],
        subtotal: 21.50,
        tax: 1.08,
        deliveryFee: 5.00,
        total: 21.50,
        orderDate: "12/8/2025",
        orderTime: "12:29:55 AM",
        timeAgo: "1 hour ago"
    },
    {
        id: "order-9",
        orderNumber: "#ORD-2023-12320",
        restaurantId: MOCK_RESTAURANT_ID,
        status: OrderStatus.Finished,
        customerName: "Bruce Wayne",
        customer: {
            name: "Bruce Wayne",
            phone: "+1 (555) 789-0123",
            address: "741 Willow Dr, New York, NY 10009"
        },
        items: [
            { id: "1", name: "Lobster", description: "", quantity: 1, price: 67.85 }
        ],
        subtotal: 67.85,
        tax: 3.39,
        deliveryFee: 5.00,
        total: 67.85,
        orderDate: "12/7/2025",
        orderTime: "11:29:55 PM",
        timeAgo: "2 hours ago"
    }
];


