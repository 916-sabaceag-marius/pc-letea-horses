import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router";
import {
  addConfigurationAPI,
  updateConfigurationAPI,
  getConfigurationByIdAPI,
  getAllCategoriesAPI,
  getCategoriesByConfigurationAPI,
} from "../../../services/configurationService";
import { jwtDecode } from "jwt-decode";

export default function AddConfigurationPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [configuration, setConfiguration] = useState({
    name: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [categories, setCategories] = useState([]);
  const [selectedCategoryIds, setSelectedCategoryIds] = useState([]);

  useEffect(() => {
    async function load() {
      setLoading(true);

      try {
        const catRes = await getAllCategoriesAPI();
        if (catRes.succeeded) {
          const raw = catRes.data;
          const list = raw?.result || raw?.data || raw || [];
          if (Array.isArray(list)) {
            const normalized = list.map((c) => ({ ...c, id: String(c.id) }));
            setCategories(normalized);
          }
        }
      } catch (err) {
      }

      if (!id) {
        setLoading(false);
        return;
      }

      let assignedIds = [];
      try {
        const byRes = await getCategoriesByConfigurationAPI(id);
        if (byRes.succeeded) {
          const rawBy = byRes.data;
          const listBy = rawBy?.result || rawBy?.data || rawBy || [];
          if (Array.isArray(listBy) && listBy.length > 0) {
            assignedIds = listBy.map((c) => String(c.id));
            setSelectedCategoryIds(assignedIds);
          }
        }
      } catch (err) {
      }

      const result = await getConfigurationByIdAPI(id);
      if (result.succeeded) {
        const data = result.data || {};
        setConfiguration(data);
        if (assignedIds.length === 0) {
          let ids = [];
          if (Array.isArray(data.categories)) ids = data.categories.map((c) => String(c.id || c));
          else if (Array.isArray(data.categoryIds)) ids = data.categoryIds.map(String);
          setSelectedCategoryIds(ids.map(String));
        }
      } else {
        setError(result.errorMessage || "Failed to load configuration");
      }

      setLoading(false);
    }

    load();
  }, [id]);

  function handleChange(e) {
    const { name, value } = e.target;
    setConfiguration((prev) => ({ ...prev, [name]: value }));
  }

  function toggleCategory(catId) {
    setSelectedCategoryIds((prev) =>
      prev.includes(catId) ? prev.filter((x) => x !== catId) : [...prev, catId]
    );
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);
    try {
      const token = localStorage.getItem("token");
      const userId = token ? jwtDecode(token).sub : undefined;

      const payload = {
        name: configuration.name,
        userId,
        categoryIds: selectedCategoryIds && selectedCategoryIds.length > 0 ? selectedCategoryIds : [],
      };

      if (!id) {
        await addConfigurationAPI(payload);
      } else {
        await updateConfigurationAPI(id, payload);
      }

      navigate("/configurations");
    } catch (err) {
      setError("Failed to save configuration.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white rounded-xl shadow-md mt-6">
      <h1 className="text-2xl font-bold mb-6">{id ? "Edit Configuration" : "Add New Configuration"}</h1>

      {error && <p className="text-red-500 mb-4">{error}</p>}

      <form onSubmit={handleSubmit} className="space-y-5">
        <div>
          <label className="block mb-1 font-medium text-gray-700">Name *</label>
          <input
            name="name"
            value={configuration.name || ""}
            onChange={handleChange}
            className="form-input w-full rounded-lg border border-gray-300 px-4 py-2"
            required
            placeholder="Enter configuration name"
          />
        </div>

        <div>
          <label className="block mb-1 font-medium text-gray-700">Categories</label>
          <div className="grid grid-cols-2 gap-2 border border-gray-300 rounded-lg p-4 bg-gray-50">
            {categories.length === 0 && (
              <div className="text-sm text-gray-500 col-span-2">No categories available</div>
            )}
            {categories.map((c) => (
              <label key={c.id} className="inline-flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={selectedCategoryIds.includes(c.id)}
                  onChange={() => toggleCategory(c.id)}
                  className="form-checkbox h-4 w-4"
                />
                <span className="text-sm">{c.name}</span>
              </label>
            ))}
          </div>
        </div>

        <div>
          <button
            type="submit"
            disabled={loading}
            className="w-full px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {loading ? "Saving..." : id ? "Update Configuration" : "Add Configuration"}
          </button>
        </div>
      </form>
    </div>
  );
}
