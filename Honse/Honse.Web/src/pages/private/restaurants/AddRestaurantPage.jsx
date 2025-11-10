import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router";
import {
  addRestaurantAPI,
  updateRestaurantAPI,
  getAllProductCategoriesAPI,
  getRestaurantByIdAPI,
} from "../../../services/restaurantService";
import { jwtDecode } from "jwt-decode";

export default function AddRestaurantPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [restaurant, setRestaurant] = useState({
    name: "",
    description: "",
    address: "",
    city: "",
    postalCode: "",
    phone: "",
    email: "",
    image: "",
    cuisineType: "",
    openingTime: "",
    closingTime: "",
    isEnabled: true,
    categoriesIds: [],
  });

  const [loading, setLoading] = useState(false);
  const [validationErrors, setValidationErrors] = useState({});
  const [generalError, setGeneralError] = useState("");

  useEffect(() => {
    async function loadCategories() {
      const res = await getAllProductCategoriesAPI();
      if (res.succeeded) setCategories(res.data.map(c => ({ ...c, checked: false })));
    }

    async function loadRestaurant() {
      if (!id) return;
      setLoading(true);
      try {
        const res = await getRestaurantByIdAPI(id);
        if (res.succeeded) {
          const r = res.data;
          setRestaurant(prev => ({
            ...prev,
            name: r.name || "",
            description: r.description || "",
            address: r.address || "",
            city: r.city || "",
            postalCode: r.postalCode || "",
            phone: r.phone || "",
            email: r.email || "",
            image: r.image || "",
            cuisineType: r.cuisineType || "",
            openingTime: r.openingTime || "",
            closingTime: r.closingTime || "",
            isEnabled: r.isEnabled ?? true,
            categoriesIds: (r.productCategories || []).map(c => c.id),
          }));
          // mark categories when already loaded
        } else {
          setGeneralError(res.errorMessage || "Failed to load restaurant.");
        }
      } catch {
        setGeneralError("Failed to load restaurant.");
      } finally {
        setLoading(false);
      }
    }

    loadCategories();
    loadRestaurant();
  }, [id]);

  useEffect(() => {
    if (categories.length === 0) return;
    setCategories(prev => prev.map(c => ({ ...c, checked: restaurant.categoriesIds?.includes(c.id) })));
  }, [restaurant.categoriesIds]);

  function handleChange(e) {
    const { name, value, type, checked } = e.target;
    setRestaurant(prev => ({ ...prev, [name]: type === "checkbox" ? checked : value }));
  }

  function handleCategoryToggle(catId) {
    setCategories(prev => prev.map(c => c.id === catId ? { ...c, checked: !c.checked } : c));
  }

  function validate() {
    const errors = {};
    if (!restaurant.name || restaurant.name.length < 3) errors.name = "Name must be at least 3 characters.";
    if (restaurant.image && !/\.(jpg|jpeg|png|webp)$/i.test(restaurant.image)) errors.image = "Image URL must end with .jpg/.png/.webp";
    return errors;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const errors = validate();
    if (Object.keys(errors).length > 0) {
      setValidationErrors(errors);
      return;
    }
    setLoading(true);
    try {
      const token = localStorage.getItem("token");
      const userId = token ? jwtDecode(token).sub : undefined;
      const payload = {
        name: restaurant.name,
        userId,
        description: restaurant.description,
        address: restaurant.address,
        city: restaurant.city,
        postalCode: restaurant.postalCode,
        phone: restaurant.phone,
        email: restaurant.email,
        image: restaurant.image,
        cuisineType: restaurant.cuisineType,
        openingTime: restaurant.openingTime,
        closingTime: restaurant.closingTime,
        isEnabled: restaurant.isEnabled,
        categoriesIds: categories.filter(c => c.checked).map(c => c.id),
      };

      if (!id) {
        await addRestaurantAPI(payload);
      } else {
        await updateRestaurantAPI(id, payload);
      }
      navigate("/restaurants");
    } catch {
      setGeneralError("Failed to save restaurant.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white dark:bg-gray-800 rounded-xl shadow-md mt-6">
      <h1 className="text-2xl font-bold mb-6">{id ? "Edit Restaurant" : "Add New Restaurant"}</h1>

      {generalError && <p className="text-red-500 mb-4">{generalError}</p>}

      <form onSubmit={handleSubmit} className="space-y-5">
        <div>
          <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Restaurant Name</label>
          <input
            name="name"
            value={restaurant.name}
            onChange={handleChange}
            className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            placeholder="Enter restaurant name"
            required
          />
          {validationErrors?.name && <p className="text-red-500 text-sm">{validationErrors.name}</p>}
        </div>

        <div>
          <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Description</label>
          <textarea
            name="description"
            value={restaurant.description}
            onChange={handleChange}
            className="form-textarea w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            rows={4}
            placeholder="Short description about the restaurant"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">City</label>
            <input
              name="city"
              value={restaurant.city}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Address</label>
            <input
              name="address"
              value={restaurant.address}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>
        </div>

        <div className="grid grid-cols-3 gap-4">
          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Phone</label>
            <input
              name="phone"
              value={restaurant.phone}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Email</label>
            <input
              name="email"
              value={restaurant.email}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Postal Code</label>
            <input
              name="postalCode"
              value={restaurant.postalCode}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>
        </div>

        <div>
          <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Image URL</label>
          <input
            name="image"
            value={restaurant.image}
            onChange={handleChange}
            className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            placeholder="https://example.com/image.jpg"
          />
          {validationErrors?.image && <p className="text-red-500 text-sm">{validationErrors.image}</p>}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Cuisine Type</label>
            <input
              name="cuisineType"
              value={restaurant.cuisineType}
              onChange={handleChange}
              className="form-input w-full rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div>
            <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Opening - Closing</label>
            <div className="flex gap-2">
              <input
                name="openingTime"
                value={restaurant.openingTime}
                onChange={handleChange}
                placeholder="09:00"
                className="form-input w-1/2 rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
              />
              <input
                name="closingTime"
                value={restaurant.closingTime}
                onChange={handleChange}
                placeholder="21:00"
                className="form-input w-1/2 rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 focus:ring-2 focus:ring-blue-500 focus:outline-none dark:bg-gray-700 dark:text-white"
              />
            </div>
          </div>
        </div>

        <div>
          <label className="block mb-1 font-medium text-gray-700 dark:text-gray-300">Categories (product categories)</label>
          <div className="grid grid-cols-2 gap-2">
            {categories.map((c) => (
              <label key={c.id} className="flex items-center gap-3 p-2 rounded border border-border-light dark:border-border-dark">
                <input type="checkbox" className="form-checkbox h-5 w-5 text-blue-600" checked={c.checked} onChange={() => handleCategoryToggle(c.id)} />
                <span className="text-sm">{c.name}</span>
              </label>
            ))}
          </div>
        </div>

        <div className="flex items-center gap-3">
          <input type="checkbox" name="isEnabled" checked={restaurant.isEnabled} onChange={handleChange} className="form-checkbox h-5 w-5 text-blue-600" />
          <label className="text-gray-700 dark:text-gray-300">Available</label>
        </div>

        <div>
          <button
            type="submit"
            disabled={loading}
            className="w-full px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition"
          >
            {loading ? "Saving..." : id ? "Update Restaurant" : "Add Restaurant"}
          </button>
        </div>
      </form>
    </div>
  );
}
