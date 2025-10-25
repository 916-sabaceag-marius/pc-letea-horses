import {createBrowserRouter} from 'react-router';
import App from '../App';
import RestaurantsListPage from '../pages/public/RestaurantListPage/RestaurantsListPage';

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App/>,
        // errorElement: <ErrorPage/>,
        children: [
            // {path: "/", element: <UnauthentificatedRoute redirectPage='/events'><LandingPage /></UnauthentificatedRoute>},
            // {path: "/events", element:<AuthentificatedRoute redirectPage='/login'><EventsPage /></AuthentificatedRoute>},
            // {path: "/events/:id", element:<AuthentificatedRoute redirectPage='/login'><EventDetailsPage /></AuthentificatedRoute>},
            // {path: "/login", element: <UnauthentificatedRoute redirectPage='/events'><LoginPage /></UnauthentificatedRoute>},
            // {path: "/register", element: <UnauthentificatedRoute redirectPage='/events'><RegisterPage /></UnauthentificatedRoute>},

            // PUBLIC PAGES - anyone can access them

            // Landing page: /public

            {path: "/public", element: <RestaurantsListPage />}

            // Restaurants list page : /public/restaurants

            // Restaurant menu page : /public/restaurants/:id
            
            // UNAUTHENTICATED PAGES - you can access them only if you aren't logged in

            // Login page: /public/login

            // Register page: /public/register

            // PRIVATE PAGES - you can access them only if you are logged in

            // Products page: /products

            // Products add page: /products/add

            // Products edit page: /products/edit

            // Restaurants page: /restaurants

            // Restaurants add page: /restaurants/add

            // Restaurants edit page: /restaurants/edit

            // {path: "*", element: <ErrorPage />}
        ]
    }
])