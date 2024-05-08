<template>
  <EmployeeVacationsTop />

  <div style="background-color: #faf7f6" class="py-6 content">
    <div class="container py-5">
      <TabView>
        <TabPanel header="Oversikt">
          <div class="text-right mb-4 mt-2">
            <div>
              <button @click="openModalForm"><i class="pi pi-plus"></i> Registrer ferie</button>
            </div>
            <EmployeeVacationsCreateModal :isOpen="isModalOpened1" @modal-close="closeModalForm">
              <template #header>Registrer ferie</template>
              <template #content>
                <EmployeeVacationsCreateForm />
              </template>
            </EmployeeVacationsCreateModal>
          </div>
          <Accordion :activeIndex="0">
            <AccordionTab header="Filter">
              <PlaceholderText />
            </AccordionTab>
          </Accordion>
        </TabPanel>
        <TabPanel header="Feriekalender">
          <PlaceholderText />
        </TabPanel>
        <TabPanel header="Ferieoverføring">
          <PlaceholderText />
        </TabPanel>
      </TabView>
      <div class="flex align-items-center my-4 justify-content-between">
        <IconField iconPosition="left">
          <InputIcon class="pi pi-search"> </InputIcon>
          <InputText v-model="value1" placeholder="Search" />
        </IconField>
        <div class="flex gap-3">
          <div>Åpne alle</div>
          <div>Skriv ut</div>
        </div>
      </div>

      <div class="vacation-results">
        <Accordion>
          <AccordionTab v-for="(employee, index) in employees" :key="index" :header="employee.name">
            <div class="employee-details">
              <table style="width: 100%">
                <thead>
                  <tr>
                    <th>Fra dato</th>
                    <th>Til dato</th>
                    <th>Status</th>
                    <th>Antall dager</th>
                    <th>Handling</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>{{ getLastVacationFrom(employee) }}</td>
                    <td>{{ getLastVacationTo(employee) }}</td>
                    <td>
                      <div :class="getStatusClass(getLastVacationStatus(employee))" class="status">
                        {{ getLastVacationStatus(employee) }}
                      </div>
                    </td>
                    <td>{{ employee.remaining }}</td>
                    <td
                      ><i class="pi pi-pencil" @click="openModalEdit" style="margin-right: 30px"></i
                      ><i class="pi pi-trash" @click="handleDelete(employee.id, getLastVacationId(employee))"></i
                    ></td>
                    <EmployeeVacationsCreateModal :isOpen="isModalOpened2" @modal-close="closeModalEdit"
                      ><template #header>{{ employee.name }}</template>
                      <template #content>
                        <div
                          :vacationId="getLastVacationId(employee)"
                          :employeeID="employee.id"
                          class="pending-vacation-container"
                        >
                          <h3
                            >Status:
                            <span>{{ getLastVacationStatus(employee) }}</span>
                          </h3>
                          <div class="pending-btn-container">
                            <button @click="rejectVacation(employee.id, getLastVacationId(employee))" class="reject-btn"
                              >Avvis ferie</button
                            >
                            <button
                              @click="approveVacation(employee.id, getLastVacationId(employee))"
                              class="approve-btn"
                              >Godkjenn ferie</button
                            >
                          </div>
                        </div>
                      </template>
                    </EmployeeVacationsCreateModal>
                  </tr>
                </tbody>
              </table>
            </div>
          </AccordionTab>
        </Accordion>
      </div>
    </div>
  </div>
  <Footer />
</template>

<script setup>
import { ref, onMounted } from "vue";
import { updateVacationStatus, fetchDataFromApi } from "./constants/getApi.js";
import { defineProps, toRefs } from "vue";
import { deleteVacationEntry } from "./constants/getApi.js";
import TabView from "primevue/tabview";
import TabPanel from "primevue/tabpanel";
import Accordion from "primevue/accordion";
import AccordionTab from "primevue/accordiontab";
import IconField from "primevue/iconfield";
import InputIcon from "primevue/inputicon";
import EmployeeVacationsTop from "./components/EmployeeVacationsTop";
import EmployeeVacationsCreateModal from "./components/EmployeeVacationsCreateModal";
import EmployeeVacationsCreateForm from "./components/EmployeeVacationsCreateForm";
import Footer from "./components/layout/FooterMenu";
import InputText from "primevue/inputtext";
import PlaceholderText from "./components/layout/PlaceholderText.vue";

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

const { vacationId } = toRefs(props);
const employees = ref([]);
const isModalOpened1 = ref(false);
const isModalOpened2 = ref(false);

const handleDelete = async (eid, vid) => {
  try {
    await deleteVacationEntry(eid, vid);
    console.log("Ferieoppføringen ble avvist og slettet.");
  } catch (error) {
    console.error("Feil ved avvisning og sletting av ferieoppføring:", error);
  }
  setTimeout(async () => {
    const { employees: fetchedEmployees } = await fetchDataFromApi();
    employees.value = fetchedEmployees;
  }, 500);
};

const rejectVacation = async (eid, vid) => {
  try {
    await deleteVacationEntry(eid, vid);
    console.log("Ferieoppføringen ble avvist og slettet.");
  } catch (error) {
    console.error("Feil ved avvisning og sletting av ferieoppføring:", error);
  }
};

const approveVacation = async (eid, vid) => {
  let status = "Approved";
  await updateStatus(eid, vid, { status: status, id: vacationId.value });
};

const updateStatus = async (employeeId, vacationId, updatedEntry) => {
  try {
    await updateVacationStatus(employeeId, vacationId, updatedEntry);
    console.log("Ferieoppføringens status ble oppdatert.");
  } catch (error) {
    console.error("Feil ved oppdatering av status:", error);
  }
};

const openModalForm = () => {
  isModalOpened1.value = true;
};

const closeModalForm = async () => {
  isModalOpened1.value = false;
  setTimeout(async () => {
    const { employees: fetchedEmployees } = await fetchDataFromApi();
    employees.value = fetchedEmployees;
  }, 500);
};

const openModalEdit = () => {
  isModalOpened2.value = true;
};

const closeModalEdit = async () => {
  isModalOpened2.value = false;
  setTimeout(async () => {
    const { employees: fetchedEmployees } = await fetchDataFromApi();
    employees.value = fetchedEmployees;
  }, 500);
};

onMounted(async () => {
  const { employees: fetchedEmployees } = await fetchDataFromApi();
  employees.value = fetchedEmployees;
});

const getLastVacationFrom = (employee) => {
  const lastVacation = employee.vacationEntries[employee.vacationEntries.length - 1];
  if (lastVacation) {
    const fromDate = lastVacation.from;
    return fromDate.replace(/\//g, ".");
  }
  return lastVacation ? lastVacation.from : "-";
};

const getLastVacationTo = (employee) => {
  const lastVacation = employee.vacationEntries[employee.vacationEntries.length - 1];
  if (lastVacation) {
    const fromDate = lastVacation.to;
    return fromDate.replace(/\//g, ".");
  }
  return lastVacation ? lastVacation.to : "-";
};

const getLastVacationId = (employee) => {
  const lastVacation = employee.vacationEntries[employee.vacationEntries.length - 1];
  return lastVacation ? lastVacation.id : null;
};

const getLastVacationStatus = (employee) => {
  const lastVacation = employee.vacationEntries[employee.vacationEntries.length - 1];
  let status = "Venter";

  if (lastVacation) {
    if (lastVacation.status === "Approved") {
      status = "Godkjent";
    } else if (lastVacation.status === "Rejected") {
      status = "Avvist";
    }
    return status;
  }
};

const getStatusClass = (status) => {
  return {
    pending: status === "Venter",
    approved: status === "Godkjent",
    rejected: status === "Avvist",
  };
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #000000;
}
</style>
