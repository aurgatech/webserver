import Devices from "../pages/Devices";
import Home from "../pages/Home";
import Settings from "../pages/Settings";

export const routes = [
  { path: "/", element: Home },
  { path: "/devices", element: Devices },
  { path: "/settings", element: Settings },
]