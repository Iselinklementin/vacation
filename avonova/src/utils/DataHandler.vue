<script>
import { ref } from "vue";

function formatDate(date) {
  const options = { year: "numeric", month: "2-digit", day: "2-digit" };
  return date.toLocaleDateString("en-GB", options);
}

export function formatEmployees(employees) {
  return employees.map((employee) => ({
    ...employee,
    vacationEntries: employee.vacationEntries.map((entry) => ({
      ...entry,
      from: formatDate(new Date(entry.from)),
      to: formatDate(new Date(entry.to)),
    })),
  }));
}

export default {
  setup() {
    const employees = ref([]);
    const countries = ref([]);

    return {
      employees,
      countries,
      formatEmployees,
    };
  },
};
</script>
