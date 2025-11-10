import { api, parseError, failure } from "./productService";

function successData(data, extra = {}) {
  return { succeeded: true, data, ...extra };
}

export async function getRestaurantsAPI({
  userId,
  searchKey,
  isActive,
  pageSize = 6,
  pageNumber = 1,
}) {
  try {
    const params = new URLSearchParams();
    if (userId) params.append("UserId", userId);
    if (searchKey) params.append("SearchKey", searchKey);
    if (isActive !== undefined) params.append("IsActive", isActive);
    params.append("PageSize", pageSize);
    params.append("PageNumber", pageNumber);
    const res = await api.get(`/api/restaurants?${params.toString()}`);
    return {
      succeeded: true,
      restaurants: res.data.result,
      totalCount: res.data.totalCount,
      pageNumber: res.data.pageNumber,
    };
  } catch (err) {
    console.error("getRestaurantsAPI failed:", err);

    const fallback = [
      {
        id: "00000000-0000-0000-0000-000000000001",
        name: "Demo Restaurant 1",
        description: "Sample description",
        address: "123 Demo St",
        city: "Demo City",
        postalCode: "00000",
        phone: "+40 000 000 000",
        email: "demo1@example.com",
        image: "https://via.placeholder.com/150",
        cuisineType: "International",
        averageRating: 4.5,
        isEnabled: true,
      },
      {
        id: "00000000-0000-0000-0000-000000000002",
        name: "Demo Restaurant 2",
        description: "Sample description",
        address: "456 Demo Ave",
        city: "Demo Town",
        postalCode: "11111",
        phone: "+40 111 111 111",
        email: "demo2@example.com",
        image: "https://via.placeholder.com/150",
        cuisineType: "Local",
        averageRating: 4.0,
        isEnabled: false,
      },
    ];

    return {
      succeeded: true,
      restaurants: fallback,
      totalCount: fallback.length,
      pageNumber: 1,
      _warning: parseError(err, "Failed to fetch restaurants"),
    };
  }
}

export async function addRestaurantAPI(restaurant) {
  try {
    const res = await api.post("/api/restaurants", restaurant);
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to add restaurant."));
  }
}

export async function updateRestaurantAPI(id, restaurant) {
  try {
    const res = await api.put(`/api/restaurants/${id}`, restaurant);
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to update restaurant."));
  }
}

export async function deleteRestaurantAPI(id) {
  try {
    const res = await api.delete(`/api/restaurants/${id}`);
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to delete restaurant."));
  }
}

export async function getRestaurantByIdAPI(id) {
  try {
    const res = await api.get(`/api/restaurants/${id}`);
    return successData(res.data);
  } catch (err) {
    return failure(parseError(err, "Failed to fetch restaurant."));
  }
}

export async function getAllProductCategoriesAPI() {
  return {
    succeeded: true,
    data: [
      { id: "c1", name: "Starters" },
      { id: "c2", name: "Main" },
      { id: "c3", name: "Desserts" },
    ],
  };
}
