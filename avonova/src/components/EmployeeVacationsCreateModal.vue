<script setup>
import { defineProps, defineEmits, ref } from "vue";
import { onClickOutside } from "@vueuse/core";

defineProps({
  isOpen: Boolean,
});

const emit = defineEmits(["modal-close"]);

const target = ref(null);
onClickOutside(target, () => emit("modal-close"));
</script>

<template>
  <div v-if="isOpen" class="modal-mask">
    <div class="modal-wrapper">
      <div class="modal-container" ref="target">
        <div class="modal-header">
          <slot name="header"></slot>
        </div>
        <div class="modal-body">
          <slot name="content"></slot>
        </div>
        <div class="modal-footer">
          <slot name="footer">
            <div style="position: relative">
              <button
                @click.stop="emit('modal-close')"
                class="btn-outlined"
                style="position: absolute; top: -50px; left: 20px"
                >Lukk</button
              >
            </div>
          </slot>
        </div>
      </div>
    </div>
  </div>
</template>
