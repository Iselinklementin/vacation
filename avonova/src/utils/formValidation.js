export function formatDates(dates) {
  return dates.map((dateString) => {
    const [year, month, day] = dateString.split("-").map(Number);
    return new Date(year, month - 1, day);
  });
}

export function validateDate(dateString) {
  if (!dateString) {
    return false;
  }

  const dateObject = new Date(dateString);
  return !isNaN(dateObject.getTime());
}

export function generateDisabledDates(invalidHolidays = []) {
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
}

export function enoughRemainingDays(startDate, endDate, remainingDays) {
  const workdays = calculateWorkdays(startDate, endDate);
  return workdays <= remainingDays;
}

export function validateDateRange(startDate, endDate) {
  const start = new Date(startDate);
  const end = new Date(endDate);
  return start < end;
}

export function calculateWorkdays(startDate, endDate) {
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
