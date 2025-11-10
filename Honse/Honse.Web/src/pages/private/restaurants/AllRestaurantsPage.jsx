import Sidebar from "../../../components/private/Sidebar";
import Header from "../../../components/private/Header";
import Filters from "../../../components/private/Filters";
import RestaurantsTable from "../../../components/private/RestaurantsTable";
import { getRestaurantsAPI, getAllProductCategoriesAPI } from "../../../services/restaurantService";
import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

export default function AllRestaurantsPage() {
  const [restaurants, setRestaurants] = useState([]);
  const [error, setError] = useState("");
  const [searchKey, setSearchKey] = useState("");
  const [isActive, setIsActive] = useState(undefined);
  const [categories, setCategories] = useState([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(6);
  const [totalCount, setTotalCount] = useState(0);

  useEffect(() => {
    async function loadCategories() {
      const result = await getAllProductCategoriesAPI();
      if (result.succeeded) setCategories(result.data);
    }

    async function loadRestaurants() {
      const token = localStorage.getItem("token");
      if (!token) return;
      const userId = jwtDecode(token).sub;
      const result = await getRestaurantsAPI({ userId, searchKey, isActive, pageSize, pageNumber });
      if (result.succeeded) {
        setRestaurants(result.restaurants);
        setTotalCount(result.totalCount);
      } else {
        setError(result.errorMessage);
      }
    }

    loadCategories();
    loadRestaurants();
  }, [searchKey, isActive, pageNumber]);

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <main className="flex-1 overflow-y-auto p-8">
        <div className="max-w-7xl mx-auto">
          <Header title="Restaurants" showAddButton={true} addPath="/restaurants/add" addText="Add Restaurant" />

          <Filters
            categoryName={""}
            setCategoryName={() => {}}
            searchKey={searchKey}
            setSearchKey={setSearchKey}
            searchPlaceholder={"Search for restaurant"}
            isActive={isActive}
            setIsActive={setIsActive}
            categories={categories.map((c) => c.name)}
          />

          {error && <p className="text-red-500 mb-4">{error}</p>}

          <RestaurantsTable restaurants={restaurants} setRestaurants={setRestaurants} />

          <div className="flex justify-center items-center mt-4 gap-2">
            <button onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))} disabled={pageNumber === 1} className="px-3 py-1 rounded bg-orange-500 hover:bg-orange-600 text-white disabled:opacity-70">Previous</button>
            <span className="px-3 py-1">Page {pageNumber} of {Math.ceil(totalCount / pageSize) || 1}</span>
            <button onClick={() => setPageNumber((prev) => prev < Math.ceil(totalCount / pageSize) ? prev + 1 : prev)} disabled={pageNumber >= Math.ceil(totalCount / pageSize)} className="px-3 py-1 rounded  bg-orange-500 hover:bg-orange-600 text-white disabled:opacity-70">Next</button>
          </div>
        </div>
      </main>
    </div>
  );
}
