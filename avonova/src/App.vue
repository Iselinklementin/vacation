<template>
  <EmployeeVacations />

  <div style="background-color: #faf7f6">
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
              <!-- <template #footer>Custom content</template> -->
            </EmployeeVacationsCreateModal>
          </div>
          <Accordion :activeIndex="0">
            <AccordionTab header="Filter">
              <p class="m-0">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et
                dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip
                ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu
                fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia
                deserunt mollit anim id est laborum.
              </p>
            </AccordionTab>
          </Accordion>
        </TabPanel>
        <TabPanel header="Feriekalender">
          <p class="m-0">
            Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem
            aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.
            Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni
            dolores eos qui ratione voluptatem sequi nesciunt. Consectetur, adipisci velit, sed quia non numquam eius
            modi.
          </p>
        </TabPanel>
        <TabPanel header="Ferieoverføring">
          <p class="m-0">
            At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti
            atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique
            sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum
            facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil
            impedit quo minus.
          </p>
        </TabPanel>
      </TabView>
      <div class="flex justify-content-between align-items-center my-4">
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
                    <th>ID</th>
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
                    <td>{{ getLastVacationId(employee) }}</td>
                    <td><i class="pi pi-pencil" @click="openModalEdit"></i><i class="pi pi-trash"></i></td>
                    <EmployeeVacationsCreateModal :isOpen="isModalOpened2" @modal-close="closeModalEdit"
                      ><template #header>Status</template>
                      <template #content
                        ><EditEmployeeStatusVacation
                          :vacationId="getLastVacationId(employee)"
                          :employeeID="employee.id"
                        ></EditEmployeeStatusVacation
                      ></template>
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
</template>

<script setup>
import { ref, onMounted } from "vue";
import TabView from "primevue/tabview";
import TabPanel from "primevue/tabpanel";
import Accordion from "primevue/accordion";
import AccordionTab from "primevue/accordiontab";
import IconField from "primevue/iconfield";
import InputIcon from "primevue/inputicon";
import EmployeeVacations from "./components/EmployeeVacations";
import EmployeeVacationsCreateModal from "./components/EmployeeVacationsCreateModal";
import EmployeeVacationsCreateForm from "./components/EmployeeVacationsCreateForm";
import EditEmployeeStatusVacation from "./components/EditEmployeeStatusVacation";
import InputText from "primevue/inputtext";
import { fetchData } from "./constants/getApi.js";

const employees = ref([]);
const countries = ref([]);

const isModalOpened1 = ref(false);
const isModalOpened2 = ref(false);

const openModalForm = () => {
  isModalOpened1.value = true;
};

const closeModalForm = () => {
  isModalOpened1.value = false;
  /*  window.location.reload(); */
};

const openModalEdit = () => {
  isModalOpened2.value = true;
};

const closeModalEdit = async () => {
  isModalOpened2.value = false;
  await fetchDataFromApi();
};

onMounted(async () => {
  await fetchDataFromApi();
});

async function fetchDataFromApi() {
  try {
    const { employees: fetchedEmployees, countries: fetchedCountries } = await fetchData();
    employees.value = formatEmployees(fetchedEmployees);
    countries.value = fetchedCountries;
    console.log(employees.value);
  } catch (error) {
    console.error("Error fetching data from API:", error);
  }
}

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

function formatEmployees(employees) {
  return employees.map((employee) => ({
    ...employee,
    vacationEntries: employee.vacationEntries.map((entry) => ({
      ...entry,
      from: formatDate(new Date(entry.from)),
      to: formatDate(new Date(entry.to)),
    })),
  }));
}

function formatDate(date) {
  const options = { year: "numeric", month: "2-digit", day: "2-digit" };
  return date.toLocaleDateString("en-GB", options);
}
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
