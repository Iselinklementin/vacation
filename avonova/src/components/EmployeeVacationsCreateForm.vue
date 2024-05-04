<template>
  <div class="selectEmployeeContainer">
    <button @click="fetchEmployees" class="btn-outlined"> <i class="pi pi-plus"></i>Legg til ansatt </button>
    <select
      v-model="selectedEmployee"
      @change="handleEmployeeChange"
      v-if="showSelect"
      @input="clearError"
      class="selectEmployee"
    >
      <option v-for="employee in employees" :key="employee.id" :value="employee.id">{{ employee.name }}</option>
    </select>
    <input type="hidden" :value="selectedEmployee ? selectedEmployee.id : null" name="selectedEmployeeId" />
  </div>

  <div id="updateVacationForm">
    <form @submit.prevent="updateVacation(selectedEmployee)">
      <div class="mt-5">
        <p v-if="selectedEmployee" style="margin-bottom: 4px; font-size: 14px"
          >Registrerer ferie for: <strong> {{ selectedEmployee.name }}</strong></p
        >
        <p v-if="selectedEmployee" style="margin: 0; font-size: 14px"
          >Antall feriedager: <strong ref="remainingDays"> {{ selectedEmployee.remaining }}</strong></p
        >
        <p v-if="selectedEmployee" style="margin: 0; font-size: 14px"
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
            @input="clearError"
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
            @input="clearError"
            :disabledDates="disabledDates"
          ></VueDatePicker>
        </div>
      </span>

      <div class="error">
        <p v-if="error">{{ error }}</p>
      </div>

      <div class="success" v-if="success">
        <p>Skjemaet ble sendt, takktakk!</p>
      </div>

      <div class="flex gap-3 justify-content-between" style="margin-top: 4rem">
        <button>Send</button>
      </div>
    </form>
  </div>
</template>

<!-- GODKJENT MÅ INN ET STED -->

<!--  <span class="form-input-wrapper flex">
        <input id="approved" class="form-input" type="checkbox" v-model="isChecked" />
        <label for="approved">Godkjent</label>
      </span> -->

<script setup>
import axios from "axios";
import { API_BASE_URL } from "../constants/api.js";
import { ref, watch } from "vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";

const error = ref(null);
const success = ref(false);
const startDateInput = ref("");
const endDateInput = ref("");
const employees = ref([]);
const showSelect = ref(false);
const selectedEmployee = ref(null);
const selectedEmployeeId = ref(null);

/* EXPORT alle funksjoner */

const disabledDatesMap = {};

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
    /*  console.log(response.data); */
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

const userId = ref(null);
const remainingDays = ref(null);
let disabledDates = ref([]);

const handleEmployeeChange = async function (event) {
  /*  const year = 2024; */
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

const generateDisabledDates = (invalidHolidays = []) => {
  const today = new Date();
  const disabledDatesArray = invalidHolidays.map((date) => new Date(date));

  for (let date = new Date(0); date < today; date.setDate(date.getDate() + 1)) {
    disabledDatesArray.push(new Date(date));
  }

  const newDates = formatDates(invalidHolidays);

  newDates.forEach((date) => {
    disabledDatesArray.push(new Date(date));
  });

  return disabledDatesArray;
};

function calculateWorkdays(startDate, endDate) {
  const weekdays = [1, 2, 3, 4, 5]; // Mandag til fredag
  let workdays = 0;
  let currentDate = new Date(startDate);

  while (currentDate <= endDate) {
    if (weekdays.includes(currentDate.getDay())) {
      workdays++;
    }
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return workdays;
}

function formatDates(dates) {
  return dates.map((dateString) => {
    const [year, month, day] = dateString.split("-").map(Number);
    return new Date(year, month - 1, day);
  });
}

function enoughRemainingDays(startDate, endDate, remainingDays) {
  const workdays = calculateWorkdays(startDate, endDate);
  return workdays <= remainingDays;
}

function validateDateRange(startDate, endDate) {
  const start = new Date(startDate);
  const end = new Date(endDate);
  return start < end;
}

function validateDate(dateString) {
  if (!dateString) {
    return false;
  }

  const dateObject = new Date(dateString);
  return !isNaN(dateObject.getTime());
}

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
  // Må få inn pending her

  try {
    const newVacationEntry = {
      id: 0,
      from: startDateInput,
      to: endDateInput,
    };

    const updatedEmployee = { ...selectedEmployee };
    if (!updatedEmployee.vacationEntries) {
      updatedEmployee.vacationEntries = [];
    }

    updatedEmployee.vacationEntries.push(newVacationEntry);

    await axios.put(`${API_BASE_URL}/employees/${userId.value}`, updatedEmployee, {
      headers: {
        "Content-Type": "application/json",
      },
    });
    success.value = true;
  } catch (error) {
    success.value = false;
    console.error("Feil ved oppdatering av ferie:", error);
  }
}
</script>
