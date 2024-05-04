<template>
  <HeaderMenu />
  <Breadcrumbs />
  <div class="container">
    <h1 class="mb-0">Ferie</h1>
    <p class="mt-0"
      >Registrer, administrer og overfør feriedager for dine ansatte. Godkjenn eller avslå ferieønsker og se oversikt
      over feriedager for din bedrift.</p
    >
  </div>
</template>

<script>
import HeaderMenu from "../components/layout/HeaderMenu";
import Breadcrumbs from "../components/layout/BreadCrumbs";

export default {
  data() {
    return {};
  },
  mounted() {},
  methods: {},
  components: {
    HeaderMenu,
    Breadcrumbs,
  },
};
</script>
