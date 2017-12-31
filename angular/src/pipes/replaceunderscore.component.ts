import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'replaceUnderscore',
    pure: true
})
export class ReplaceUnderscore implements PipeTransform {
  transform(value: string): string {
    let newValue = value.replace('_', '-');
    return `${newValue}`;
  }
}
