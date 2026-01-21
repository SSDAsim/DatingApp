import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'age',
})
export class AgePipe implements PipeTransform {

  transform(value: string): number {
    const today = new Date();
    const dob = new Date(value);

    let age = today.getFullYear() - dob.getFullYear();

    const monthDiff = today.getMonth() - dob.getMonth();
    // month difference must be 0 or less then 0

    // if month difference is 0 then calculate that the current day must be less then the birth day
    if (monthDiff < 0 || (monthDiff == 0 && today.getDate() < dob.getDate())) {
      age--;
    }

    return age;
  }

}
