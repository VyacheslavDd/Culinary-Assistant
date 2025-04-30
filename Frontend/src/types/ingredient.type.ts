import { Measure } from './measure.enum';

export type Ingredient = {
    numericValue: number;
    name: string;
    measure: Measure;
}