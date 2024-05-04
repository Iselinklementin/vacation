import axios from "axios";
import { API_BASE_URL } from "./api";

export const fetchData = async () => {
  try {
    const [employeesResponse, countriesResponse] = await Promise.all([
      axios.get(`${API_BASE_URL}/employees`),
      axios.get(`${API_BASE_URL}/country`),
    ]);
    return { employees: employeesResponse.data, countries: countriesResponse.data };
  } catch (error) {
    console.error("Error fetching data:", error);
    throw error;
  }
};
