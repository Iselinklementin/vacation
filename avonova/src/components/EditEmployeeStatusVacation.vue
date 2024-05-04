<template>
  <div>
    <h2>Status: <span>Venter</span></h2>
    <button @click="rejectVacation">Avvis</button>
    <button @click="approveVacation">Godkjenn</button>
  </div>
</template>

<script setup>
import { defineProps, toRefs } from "vue";
import axios from "axios";
/* import API_BASE_URL from "../constants/api.js"; */

const props = defineProps({
  vacationId: {
    type: Number,
    required: true,
  },
  employeeID: {
    type: Number,
    required: true,
  },
});

const { vacationId, employeeID } = toRefs(props);

const rejectVacation = async () => {
  let status = "Rejected";
  await updateVacationStatus(employeeID.value, vacationId.value, { status: status, id: vacationId.value });
};

const approveVacation = async () => {
  let status = "Approved";
  await updateVacationStatus(employeeID.value, vacationId.value, { status: status, id: vacationId.value });
};

const updateVacationStatus = async (employeeId, vacationId, updatedEntry) => {
  try {
    const response = await axios.put(
      `https://localhost:7191/api/employees/${employeeId}/vacation/${vacationId}`,
      updatedEntry
    );

    console.log("Ferieoppføringen ble oppdatert:", response.data);
    return response.data;
  } catch (error) {
    console.error("Feil ved oppdatering av status:", error);
    throw error;
  }
};
</script>
