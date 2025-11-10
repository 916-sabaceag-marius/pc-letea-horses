import { useState } from "react";
import { useNavigate } from "react-router";
import { deleteRestaurantAPI, updateRestaurantAPI } from "../../services/restaurantService";

export default function RestaurantsTable({ restaurants = [], setRestaurants }) {
  const [deleteTarget, setDeleteTarget] = useState(null);
  const navigate = useNavigate();

  async function handleToggle(id) {
    const r = restaurants.find(x => x.id === id);
    if (!r) return;
    const updated = { ...r, isEnabled: !r.isEnabled };
    setRestaurants(restaurants.map(x => x.id === id ? updated : x));

    const result = await updateRestaurantAPI(id, { isEnabled: updated.isEnabled });
    if (!result.succeeded) {
      alert(result.errorMessage || "Failed to update status");
      setRestaurants(restaurants);
    }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    const result = await deleteRestaurantAPI(deleteTarget.id);
    if (result.succeeded) {
      setRestaurants(prev => prev.filter(r => r.id !== deleteTarget.id));
    } else {
      alert(result.errorMessage || "Failed to delete restaurant");
    }
    setDeleteTarget(null);
  }

  return (
    <div className="bg-white dark:bg-surface-dark rounded-xl shadow-sm overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm text-left">
          <thead className="bg-background-light dark:bg-background-dark text-xs uppercase">
            <tr>
              <th className="p-4 w-12"></th>
              <th className="px-6 py-3">Image</th>
              <th className="px-6 py-3">Name</th>
              <th className="px-6 py-3">City</th>
              <th className="px-6 py-3">Phone</th>
              <th className="px-6 py-3">Cuisine</th>
              <th className="px-6 py-3">Rating</th>
              <th className="px-6 py-3">Status</th>
              <th className="px-6 py-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {restaurants.length === 0 && (
              <tr>
                <td colSpan="9" className="text-center py-6 text-gray-500">No restaurants found</td>
              </tr>
            )}

            {restaurants.map((r) => (
              <tr key={r.id} className="border-b border-border-light dark:border-border-dark">
                <td className="p-4"></td>
                <td className="px-6 py-3">
                  <img src={r.image} alt={r.name} className="h-12 w-12 rounded object-cover" />
                </td>
                <td className="px-6 py-3 font-medium">{r.name}</td>
                <td className="px-6 py-3">{r.city}</td>
                <td className="px-6 py-3">{r.phone}</td>
                <td className="px-6 py-3">{r.cuisineType}</td>
                <td className="px-6 py-3">{r.averageRating ?? "-"}</td>
                <td className="px-6 py-3">
                  <label className="cursor-pointer">
                    <input
                      type="checkbox"
                      checked={r.isEnabled}
                      onChange={() => handleToggle(r.id)}
                      className="sr-only peer"
                    />
                    <div className="w-10 h-5 bg-gray-300 rounded-full peer-checked:bg-blue-600 transition relative">
                      <div className="absolute top-[2px] left-[2px] w-4 h-4 bg-white rounded-full transition peer-checked:translate-x-5" />
                    </div>
                  </label>
                </td>
                <td className="px-6 py-3 space-x-3">
                  <button onClick={() => navigate(`/restaurants/edit/${r.id}`)} className="text-blue-600 hover:underline">
                    <span className="material-symbols-outlined text-lg">edit</span>
                  </button>
                  <button onClick={() => setDeleteTarget(r)} className="text-red-600 hover:underline">
                    <span className="material-symbols-outlined text-lg">delete</span>
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {deleteTarget && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center">
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-xl w-[350px]">
            <h2 className="text-lg font-semibold mb-2">Confirm Delete</h2>
            <p className="text-sm text-gray-500 mb-4">Are you sure you want to delete <strong>{deleteTarget.name}</strong>?</p>
            <div className="flex justify-end gap-3">
              <button onClick={() => setDeleteTarget(null)} className="px-4 py-2 rounded bg-gray-200 hover:bg-gray-300">Cancel</button>
              <button onClick={confirmDelete} className="px-4 py-2 rounded bg-red-600 text-white hover:bg-red-700">Delete</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
