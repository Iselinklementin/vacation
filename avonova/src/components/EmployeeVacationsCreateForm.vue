<template>
  <div class="selectEmployeeContainer">
    <select
      v-model="selectedEmployee"
      @change="handleEmployeeChange"
      @input="clearError"
      class="selectEmployee select-box"
      ref="select"
    >
      <option value="null" disabled>Velg ansatt</option>
      <option v-for="employee in employees" :key="employee.id" :value="employee.id" @click="handleSelect">{{
        employee.name
      }}</option>
    </select>

    <div class="selectEmployee-container">
      <span class="selectedEmployee" v-if="selectedEmployee">{{ selectedEmployee.name }}</span>
    </div>

    <input type="hidden" :value="selectedEmployee ? selectedEmployee.id : null" name="selectedEmployeeId" />
  </div>
  <div style="height: 30px"></div>

  <div id="updateVacationForm" style="position: relative">
    <form @submit.prevent="updateVacation(selectedEmployee)">
      <div class="mt-5">
        <p v-if="selectedEmployee" style="margin-bottom: 4px; font-size: 14px"
          >Registrerer ferie for: <strong> {{ selectedEmployee.name }}</strong></p
        >
        <p v-if="selectedEmployee" style="margin: 0; font-size: 14px"
          >Antall feriedager: <strong ref="remainingDays"> {{ selectedEmployee.remaining }}</strong></p
        >
        <p v-if="selectedEmployee" style="margin: 0; margin-top: 3px; font-size: 14px"
          >Bosted: <strong> {{ selectedEmployee.countryCode }}</strong></p
        >
      </div>

      <span class="form-input-wrapper flex flex-column">
        <label for="dateFrom">Fra dato:</label>
        <div class="calendar-container">
          <VueDatePicker
            type="date"
            v-model="startDateInput"
            format="yyyy/MM/dd"
            value-format="yyyy-MM-dd"
            :enable-time-picker="false"
            :typeable="true"
            @focus="clearError"
            :disabledDates="disabledDates"
          ></VueDatePicker>
        </div>
      </span>

      <span class="form-input-wrapper flex flex-column">
        <label for="dateTo">T.o.m dato:</label>
        <div class="calendar-container">
          <VueDatePicker
            type="date"
            v-model="endDateInput"
            format="yyyy/MM/dd"
            value-format="yyyy-MM-dd"
            :enable-time-picker="false"
            :typeable="true"
            @focus="clearError"
            :disabledDates="disabledDates"
          ></VueDatePicker>
        </div>
      </span>

      <div class="error">
        <p v-if="error">{{ error }}</p>
      </div>

      <div class="success" v-if="success">
        <p>Registrering fullført</p>
      </div>

      <div class="flex gap-3 justify-content-between" style="margin-top: 4rem">
        <button type="submit" style="position: absolute; right: 0; bottom: -15px" :disabled="formSubmitted"
          >Send</button
        >
      </div>
    </form>
  </div>
</template>

<script setup>
import axios from "axios";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import { API_BASE_URL } from "../constants/api.js";
import { ref, watch, onMounted } from "vue";
import {
  generateDisabledDates,
  calculateWorkdays,
  enoughRemainingDays,
  validateDateRange,
  validateDate,
} from "../utils/formValidation";

const error = ref(null);
const success = ref(false);
const startDateInput = ref("");
const endDateInput = ref("");
const employees = ref([]);
const showSelect = ref(false);
const selectedEmployee = ref(null);
const selectedEmployeeId = ref(null);
const select = ref(null);
const formSubmitted = ref(false);

const disabledDatesMap = {};
const userId = ref(null);
const remainingDays = ref(null);
let disabledDates = ref([]);

const loadDisabledDatesForCountry = async (countryCode) => {
  if (!disabledDatesMap[countryCode]) {
    try {
      const year = 2024;
      const holidays = await fetchHolidays(year, countryCode);
      if (holidays) {
        disabledDatesMap[countryCode] = generateDisabledDates(holidays.map((holiday) => holiday.date));
      } else {
        disabledDatesMap[countryCode] = [];
      }
    } catch (error) {
      console.error(`Error fetching holidays for ${countryCode}:`, error);
      disabledDatesMap[countryCode] = [];
    }
  }
};

async function fetchEmployees() {
  try {
    showSelect.value = true;
    const response = await axios.get(`${API_BASE_URL}/employees`);
    employees.value = response.data;
  } catch (error) {
    console.error("Feil ved henting av ansatte:", error);
  }
}

async function fetchHolidays(year, countryCode) {
  try {
    const response = await fetch(`https://date.nager.at/api/v3/publicholidays/${year}/${countryCode}`);
    if (!response.ok) {
      throw new Error("Mangler landskode eller årstall");
    }
    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching holidays:", error);
    return null;
  }
}

const handleEmployeeChange = async function (event) {
  const selectedId = event.target.value;
  selectedEmployee.value = employees.value.find((employee) => employee.id === parseInt(selectedId));
  selectedEmployeeId.value = selectedEmployee.value ? selectedEmployee.value.id : null;
  remainingDays.value = selectedEmployee.value ? selectedEmployee.value.remaining : null;
  let countryCode = selectedEmployee.value ? selectedEmployee.value.countryCode : null;

  if (countryCode) {
    await loadDisabledDatesForCountry(countryCode);
    disabledDates.value = disabledDatesMap[countryCode];
  } else {
    disabledDates.value = [];
  }
};

watch(selectedEmployeeId, (inputValue) => {
  userId.value = inputValue;
});

onMounted(() => {
  fetchEmployees();
});

function clearError() {
  error.value = null;
}

function validateForm(selectedEmployee) {
  if (!selectedEmployee) {
    error.value = "Velg en ansatt før du sender skjemaet.";
    return false;
  }

  if (!validateDate(startDateInput.value) || !validateDate(endDateInput.value)) {
    error.value = "Vennligst velg gyldige datoer.";
    return false;
  }

  if (!validateDateRange(startDateInput.value, endDateInput.value)) {
    error.value = "Startdatoen må være før sluttdatoen.";
    return false;
  }

  if (!enoughRemainingDays(startDateInput.value, endDateInput.value, selectedEmployee.remaining)) {
    error.value = "Du har ikke nok feriedager igjen for denne perioden.";
    return false;
  }

  return true;
}

async function updateVacation(selectedEmployee) {
  if (!validateForm(selectedEmployee)) {
    return;
  }

  const workdays = calculateWorkdays(startDateInput.value, endDateInput.value);

  try {
    const newVacationEntry = {
      id: 0,
      from: startDateInput,
      to: endDateInput,
      status: "pending",
    };

    const updatedEmployee = { ...selectedEmployee };
    if (!updatedEmployee.vacationEntries) {
      updatedEmployee.vacationEntries = [];
    }

    updatedEmployee.vacationEntries.push(newVacationEntry);
    updatedEmployee.remaining -= workdays;

    await axios.put(`${API_BASE_URL}/employees/${userId.value}`, updatedEmployee, {
      headers: {
        "Content-Type": "application/json",
      },
    });
    success.value = true;
    formSubmitted.value = true;
  } catch (error) {
    success.value = false;
    console.error("Feil ved oppdatering av ferie:", error);
  }
}
</script>

<style scoped>
.selectEmployee-container {
  position: relative;
}

.selectedEmployee {
  position: absolute;
  position: absolute;
  left: 18px;
  top: 8px;
  font-size: 14px;
  color: #01332a;
}

.select-box {
  width: 200px;
  padding: 8px;
  border-radius: 16px;
  color: transparent;
  padding: 6px 16px;
  background: transparent;
  border-color: #01332a;
  /*  color: #b4e0d1; */
  color: #01332a;
  border-radius: 30px;
  font-size: 14px;
}
</style>
