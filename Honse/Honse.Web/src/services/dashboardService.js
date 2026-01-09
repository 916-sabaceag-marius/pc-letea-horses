import { api, failure, parseError, successData } from "./productService";

const mockStats = {
  totalRevenue: 12450,
  newOrders: 230,
  completedOrders: 215,
  startDate: "2026-01-01T00:00:00.000Z",
  endDate: "2026-01-01T00:00:00.000Z"
};


const now = new Date();
let labels = [];
let data = [];

// if (range === 'lastDay') {
//   labels = Array.from({ length: 24 }, (_, i) => `${i}:00`);
//   data = labels.map(() => Math.floor(Math.random() * 1000 + 200));
// } else {
//   const days = range === 'lastWeek' ? 7 : range === 'lastMonth' ? 30 : 90;
//   labels = Array.from({ length: days }, (_, i) => {
//     const d = new Date(now);
//     d.setDate(now.getDate() - (days - i - 1));
//     return `${d.getMonth() + 1}/${d.getDate()}`;
//   });
//   data = labels.map(() => Math.floor(Math.random() * 1000 + 500));
// }

// Sales by category mock
const categories = [
  { name: 'Burgers', value: 16 },
  { name: 'Pizza', value: 12 },
  { name: 'Salads', value: 23 },
  { name: 'Drinks', value: 34 },
  { name: "Main Course", value: 8 },
  { name: "Combos", value: 24 },
  { name: 'Pasta', value: 7 },
  { name: "Souces", value: 18 },
  { name: "Desserts", value: 15 },
  { name: "Kids", value: 10 },


];

const totalSales = categories.reduce((sum, cat) => sum + cat.value, 0);
const categoriesWithPercentMock = categories.map(cat => ({
  ...cat,
  percent: ((cat.value / totalSales) * 100).toFixed(1)
}));


const topProductsMock = [
  { name: 'Classic Cheeseburger', category: 'Burgers', sales: 19, img: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDOfj7HTvzcvPeUQdvMQ0S6llsJoOElcY-gB0-igGLfiSXkjdJ44bg5e3z1URZ7sjeu_GgZJyZubY92s2mBufyvcGvI_f0hx8tsG7Ylw296drMmGKEfkMv8VjK426VxwXkW-bCfoqv7P3OcsSQ6pugHWjtczvhHw7kQjhV-gFgTd0A-X6MCd6njdIsZGeK8yVJHZdp4rmaXLd5EGtxH7pUtGBxJbi1st3Rn8JGhTF8aRxBDrQ0ebxttGfLpXdPwsvdRmxnX5fF_w8uH' },
  { name: 'Pepperoni Pizza', category: 'Pizza', sales: 16, img: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCYFend0gQJcI-3S2Qs9Yeui9sy9L3g0oqHFqNuJE9sVzisvkNYdkKrM_sGWba1wVYC2c2Ui1ZaJLq3bDEQWsCCe0AlP0BdyWc5erbskBE0aFVE-_HZJ1AcXxcZSC37nbE4w_ECY5UJUHxBUamQ8VfrhyLjkIKMx-1rjV1lJghuZlCl9rl5meFlL7BcNW02HLg8PqKEYtm33s1_6mvAerNktpKp7HcXFzy9zjd1dmHLnSdb2MWZ853X-GkNTt2QvsOUB_sWdOS6VXfV' },
  { name: 'Caesar Salad', category: 'Salads', sales: 15, img: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDaj6eDVO1wiEGM2Ja_eXyX6eboOAkjFJLgL3iI_fBv_r2yICSBTl-Q2ITZe0XDWf6FAQQhvBsgE9s2YY0ERfLbnANw0jKwQmCBuPasjGeH95KHGDFCbIlqwm32Fb8VePZxEuVDsaWuLTUwCJfts66oQFqnt6pMx3Gn7DWNS-2vYY1VWSEYZ7ekIyaN7SNG5oPLJyrgQTLGubOaT0fpsT_7YSrNrvyWuEw1__be-wdJwA7XDaUCoJlen8LJa9B8jKIDYMGiZq2XHRG-' },
  { name: 'Prosciutto Pizza', category: 'Pizza', sales: 13, img: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCYFend0gQJcI-3S2Qs9Yeui9sy9L3g0oqHFqNuJE9sVzisvkNYdkKrM_sGWba1wVYC2c2Ui1ZaJLq3bDEQWsCCe0AlP0BdyWc5erbskBE0aFVE-_HZJ1AcXxcZSC37nbE4w_ECY5UJUHxBUamQ8VfrhyLjkIKMx-1rjV1lJghuZlCl9rl5meFlL7BcNW02HLg8PqKEYtm33s1_6mvAerNktpKp7HcXFzy9zjd1dmHLnSdb2MWZ853X-GkNTt2QvsOUB_sWdOS6VXfV' },
  { name: 'Classic Salad', category: 'Salads', sales: 10, img: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDaj6eDVO1wiEGM2Ja_eXyX6eboOAkjFJLgL3iI_fBv_r2yICSBTl-Q2ITZe0XDWf6FAQQhvBsgE9s2YY0ERfLbnANw0jKwQmCBuPasjGeH95KHGDFCbIlqwm32Fb8VePZxEuVDsaWuLTUwCJfts66oQFqnt6pMx3Gn7DWNS-2vYY1VWSEYZ7ekIyaN7SNG5oPLJyrgQTLGubOaT0fpsT_7YSrNrvyWuEw1__be-wdJwA7XDaUCoJlen8LJa9B8jKIDYMGiZq2XHRG-' },
];
function getDateDiff(startDate, endDate) {
  const start = new Date(startDate);
  const end = new Date(endDate);

  // If same day, return hours difference
  if (start.toDateString() === end.toDateString()) {
    return { unit: "hours", length: 24 };
  }

  // Otherwise, return number of days difference
  const diffTime = end - start;
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
  return { unit: "days", length: diffDays };
}

function generateTimeline(startDate, endDate) {
  const { unit, length } = getDateDiff(startDate, endDate);
  const now = new Date(startDate);
  const timeline = [];

  if (unit === "hours") {
    for (let i = 0; i < 24; i++) {
      timeline.push({ label: `${i}:00`, sales: Math.floor(Math.random() * 30 + 5) });
    }
  } else {
    for (let i = 0; i < length; i++) {
      const d = new Date(startDate);
      d.setDate(d.getDate() + i);
      const label = `${d.getMonth() + 1}/${d.getDate()}`;
      timeline.push({ label, sales: Math.floor(Math.random() * 100 + 20) });
    }
  }

  return { timeline, unit };
}

function generateStats(startDate, endDate) {
  const { length } = getDateDiff(startDate, endDate);
  return {
    totalRevenue: 500 * length + Math.floor(Math.random() * 500),
    newOrders: 20 * length + Math.floor(Math.random() * 10),
    completedOrders: 15 * length + Math.floor(Math.random() * 10),
    startDate,
    endDate
  };
}
function generateTopProducts() {
  return topProductsMock
    .sort(() => Math.random() - 0.5)
    .map(p => ({ ...p, sales: p.sales + Math.floor(Math.random() * 10) }));
}

//return { stats: mockStats, lineChart: { labels, data }, categoriesWithPercentMock, topProductsMock };




export async function getTotalRevenueAPI(restaurantId, startDate, endDate) {
  try {
    const res = await api.get(`/api/dashboard/${restaurantId}/revenue`, {
      params: { startDate, endDate }
    });
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch total revenue"));
  }
}

export async function getNewOrdersAPI(restaurantId, startDate, endDate) {
  try {
    const res = await api.get(`/api/dashboard/${restaurantId}/orders/new`, {
      params: { startDate, endDate }
    });
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch new orders"));
  }
}

export async function getCompletedOrdersAPI(restaurantId, startDate, endDate) {
  try {
    const res = await api.get(`/api/dashboard/${restaurantId}/orders/completed`, {
      params: { startDate, endDate }
    });
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch completed orders"));
  }
}

export async function getSalesTimelineAPI(restaurantId, startDate, endDate) {
  try {
    const sameDay = new Date(startDate).toDateString() === new Date(endDate).toDateString();

    const endpoint = sameDay
      ? `/api/dashboard/${restaurantId}/sales/timeline/hours`
      : `/api/dashboard/${restaurantId}/sales/timeline/days`;

    const res = await api.get(endpoint, {
      params: { startDate, endDate }
    });

    return successData({
      mode: sameDay ? "hours" : "days",
      timeline: res.data
    });
  } catch (err) {
    return failure(parseError(err, "Failed to fetch sales timeline"));
  }
}

// Categories + percentages
export async function getCategorySalesAPI(restaurantId, startDate, endDate) {
  try {
    const res = await api.get(`/api/dashboard/${restaurantId}/sales/categories`, {
      params: { startDate, endDate }
    });
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch category sales"));
  }
}

// Top products
export async function getTopProductsAPI(restaurantId, startDate, endDate) {
  try {
    const res = await api.get(`/api/dashboard/${restaurantId}/sales/top-products`, {
      params: { startDate, endDate }
    });
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch best selling products"));
  }
}

// Combined dashboard loader
export async function getDashboardDataAPI(restaurantId, startDate, endDate) {
  try {
    // const [
    //     revenue,
    //     newOrders,
    //     completedOrders,
    //     timelineRes,
    //     categoryRes,
    //     productsRes
    // ] = await Promise.all([
    //     getTotalRevenueAPI(restaurantId, startDate, endDate),
    //     getNewOrdersAPI(restaurantId, startDate, endDate),
    //     getCompletedOrdersAPI(restaurantId, startDate, endDate),
    //     getSalesTimelineAPI(restaurantId, startDate, endDate),
    //     getCategorySalesAPI(restaurantId, startDate, endDate),
    //     getTopProductsAPI(restaurantId, startDate, endDate)
    // ]);

    // if (!revenue.succeeded || !newOrders.succeeded || !completedOrders.succeeded ||  !timelineRes.succeeded || !categoryRes.succeeded || !productsRes.succeeded)
    //     return failure("Failed to load dashboard stats");

    // const stats = {
    //     totalRevenue: revenue.data.totalRevenue,
    //     newOrders: newOrders.data.newOrders,
    //     completedOrders: completedOrders.data.completedOrders,
    //     startDate,
    //     endDate
    // };

    // // Compute donut percentages
    // const categories = categoryRes.data || [];
    // const totalSales = categories.reduce((sum, c) => sum + c.sales, 0);

    // const categoriesWithPercent = categories.map(c => ({
    //     name: c.name,
    //     sales: c.sales,
    //     percent: ((c.sales / totalSales) * 100).toFixed(1)
    // }));

    // return successData({
    //     stats,
    //     lineChart: {
    //         mode: timelineRes.data.mode,
    //         timeline: timelineRes.data.timeline
    //     },
    //     categories: categoriesWithPercent,
    //     topProducts: productsRes.data
    // });

    // return successData({
    //   stats: mockStats,
    //   lineChart: {
    //     mode: "hours",
    //     timeline: [
    //       { label: "10:00", sales: 7 },
    //       { label: "11:00", sales: 12 },
    //       { label: "12:00", sales: 17 },
    //       { label: "13:00", sales: 23 },
    //       { label: "14:00", sales: 5 },
    //       { label: "15:00", sales: 24 },
    //       { label: "16:00", sales: 20 },
    //       { label: "18:00", sales: 10 },
    //       { label: "19:00", sales: 9 },
    //       { label: "20:00", sales: 12 },
    //     ],
    //   },
    //   categories: categoriesWithPercentMock,
    //   topProducts: topProductsMock
    // });
     const stats = generateStats(startDate, endDate);
    const { timeline, unit } = generateTimeline(startDate, endDate);
    const topProducts = generateTopProducts();

    return successData({
      stats,
      lineChart: {
        mode: unit,
        timeline
      },
      categories: categoriesWithPercentMock,
      topProducts
    });
  } catch (err) {
    return failure(parseError(err, "Failed to load dashboard"));
  }
}

