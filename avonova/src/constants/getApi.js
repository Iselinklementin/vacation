import axios from "axios";
import { formatEmployees } from "../utils/DataHandler.vue";
import { API_BASE_URL } from "./api";

export const fetchData = async () => {
  try {
    const countriesResponse = await axios.get(`${API_BASE_URL}/country`);
    const countries = countriesResponse.data;

    const employeesResponse = await axios.get(`${API_BASE_URL}/employees`);
    const employees = employeesResponse.data;

    return { countries, employees };
  } catch (error) {
    console.error("Error fetching data:", error);
    throw error;
  }
};

export const fetchDataFromApi = async () => {
  try {
    const { employees: fetchedEmployees, countries: fetchedCountries } = await fetchData();
    const formattedEmployees = formatEmployees(fetchedEmployees);
    console.log(formattedEmployees);
    return { employees: formattedEmployees, countries: fetchedCountries };
  } catch (error) {
    console.error("Error fetching data from API:", error);
    throw error;
  }
};

export const fetchVacationEntry = async (employeeId, vacationId) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/employees/${employeeId}/vacation/${vacationId}`);
    return response.data;
  } catch (error) {
    console.error("Error fetching vacation entry:", error);
    throw error;
  }
};

export const updateVacationEntry = async (employeeId, vacationId, updatedEntry) => {
  try {
    const response = await axios.put(`${API_BASE_URL}/employees/${employeeId}/vacation/${vacationId}`, updatedEntry);
    return response.data;
  } catch (error) {
    console.error("Error updating vacation entry:", error);
    throw error;
  }
};

export const deleteVacationEntry = async (employeeId, vacationId) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/employees/${employeeId}/vacation/${vacationId}`);
    return response.data;
  } catch (error) {
    console.error("Error deleting vacation entry:", error);
    throw error;
  }
};

export const updateVacationStatus = async (employeeId, vacationId, updatedEntry) => {
  console.log(employeeId, vacationId, updatedEntry);
  try {
    const response = await axios.put(`${API_BASE_URL}/employees/${employeeId}/vacation/${vacationId}`, updatedEntry);
    console.log("Ferieoppføringen ble oppdatert:", response.data);
    fetchDataFromApi();
  } catch (error) {
    console.error("Feil ved oppdatering av status:", error);
    throw error;
  }
};
