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

const MOCK_CATEGORIES = [
  { id: "cat-1", name: "Pizza & Pasta" },
  { id: "cat-2", name: "Burgers & Sandwiches" },
  { id: "cat-3", name: "Salads" },
  { id: "cat-4", name: "Desserts" },
  { id: "cat-5", name: "Beverages" },
  { id: "cat-6", name: "Appetizers" },
];

let MOCK_CONFIGURATIONS = [
  {
    id: "config-1",
    userId: "user-1",
    name: "Italian Configuration",
    categoryIds: ["cat-1"],
    createdDate: new Date(2025, 11, 1).toISOString(),
  },
  {
    id: "config-2",
    userId: "user-1",
    name: "American Configuration",
    categoryIds: ["cat-2", "cat-4"],
    createdDate: new Date(2025, 11, 5).toISOString(),
  },
  {
    id: "config-3",
    userId: "user-1",
    name: "Healthy Configuration",
    categoryIds: ["cat-3", "cat-5"],
    createdDate: new Date(2025, 11, 8).toISOString(),
  },
];


let MOCK_RESTAURANTS_CONFIG = {
  "restaurant-1": "config-1", 
  "restaurant-2": "config-2", 
};


const delay = (ms = 300) => new Promise((resolve) => setTimeout(resolve, ms));

export async function getConfigurationsAPI({ userId, searchKey = "", pageNumber = 1, pageSize = 10 }) {
  await delay();
  
  let filtered = MOCK_CONFIGURATIONS.filter((c) => c.userId === userId);
  
  if (searchKey) {
    filtered = filtered.filter((c) =>
      c.name.toLowerCase().includes(searchKey.toLowerCase())
    );
  }
  
  const totalCount = filtered.length;
  const startIndex = (pageNumber - 1) * pageSize;
  const paginatedConfigs = filtered.slice(startIndex, startIndex + pageSize);
  
  return successData(paginatedConfigs, {
    configurations: paginatedConfigs,
    totalCount: totalCount,
  });
}

export async function getConfigurationByIdAPI(id) {
  await delay();
  
  const config = MOCK_CONFIGURATIONS.find((c) => c.id === id);
  if (!config) {
    return failure("Configuration not found");
  }
  
  return successData(config);
}

export async function getAllCategoriesAPI() {
  try {
    const response = await api.get("/api/productCategory/all");
    return successData(response.data);
  } catch (err) {
    const errorMsg = parseError(err, "Failed to load categories");
    return failure(errorMsg);
  }
}

export async function getCategoriesByConfigurationAPI(configurationId) {
  await delay();
  
  const config = MOCK_CONFIGURATIONS.find((c) => c.id === configurationId);
  if (!config) {
    return failure("Configuration not found");
  }
  
  const categories = MOCK_CATEGORIES.filter((cat) =>
    config.categoryIds.includes(cat.id)
  );
  
  return successData(categories);
}


export async function getAllConfigurationsAPI() {
  await delay();
  return successData(MOCK_CONFIGURATIONS);
}

export async function addConfigurationAPI(payload) {
  await delay();
  
  const newConfig = {
    id: `config-${Date.now()}`,
    ...payload,
    createdDate: new Date().toISOString(),
  };
  
  MOCK_CONFIGURATIONS.push(newConfig);
  return successData(newConfig);
}


export async function updateConfigurationAPI(id, payload) {
  await delay();
  
  const index = MOCK_CONFIGURATIONS.findIndex((c) => c.id === id);
  if (index === -1) {
    return failure("Configuration not found");
  }
  
  const updated = {
    ...MOCK_CONFIGURATIONS[index],
    ...payload,
  };
  
  MOCK_CONFIGURATIONS[index] = updated;
  return successData(updated);
}


export async function deleteConfigurationAPI(id) {
  await delay();
  
  const index = MOCK_CONFIGURATIONS.findIndex((c) => c.id === id);
  if (index === -1) {
    return failure("Configuration not found");
  }
  
  const restaurantsUsingConfig = Object.values(MOCK_RESTAURANTS_CONFIG).filter(
    (configId) => configId === id
  );
  
  if (restaurantsUsingConfig.length > 0) {
    return failure(
      `Cannot delete this configuration. ${restaurantsUsingConfig.length} restaurant(s) are using it.`
    );
  }
  
  MOCK_CONFIGURATIONS.splice(index, 1);
  return successData({ success: true });
}
