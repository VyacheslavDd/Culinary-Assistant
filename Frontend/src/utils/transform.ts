import { Category, CookingDifficulty, Measure } from 'types';
import { format, parseISO } from 'date-fns';
import { ru } from 'date-fns/locale';

export function transformDifficulty(difficulty: CookingDifficulty): string {
    switch (difficulty) {
        case CookingDifficulty.easy:
            return 'Легко';
        case CookingDifficulty.medium:
            return 'Средне';
        case CookingDifficulty.hard:
            return 'Сложно';
        default:
            return 'Легко';
    }
}

export function transformRating(rating: number | string): string {
    const num = typeof rating === 'string' ? parseFloat(rating) : rating;

    if (isNaN(num)) {
        return '0.0';
    }
    const rounded = Math.round(num * 10) / 10;
    return rounded.toFixed(1);
}

export function transformCookingTime(minutes: number): string {
    if (minutes < 60) {
        return `${minutes} мин`;
    }

    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;

    return `${hours} ч ${remainingMinutes} мин`;
}

export function transformCategory(category: Category): string {
    switch (category) {
        case Category.breakfast:
            return 'Завтрак';
        case Category.lunch:
            return 'Обед';
        case Category.dinner:
            return 'Ужин';
        case Category.mainCourse:
            return 'Основное блюдо';
        case Category.soup:
            return 'Суп';
        case Category.pasta:
            return 'Паста';
        case Category.sideDish:
            return 'Гарнир';
        case Category.salad:
            return 'Салат';
        case Category.dessert:
            return 'Десерт';
        case Category.drink:
            return 'Напиток';
        case Category.pastry:
            return 'Выпечка';
        case Category.sauce:
            return 'Соус';
        case Category.appetizer:
            return 'Закуска';
        default:
            return 'Блюдо';
    }
}

export function transformMeasure(measure: Measure): string {
    switch (measure) {
        case Measure.gram:
            return 'г';
        case Measure.kilogram:
            return 'кг';
        case Measure.liter:
            return 'л';
        case Measure.milliliter:
            return 'мл';
        case Measure.piece:
            return 'шт';
        case Measure.pinch:
            return 'щепотка';
        case Measure.teaspoon:
            return 'ч.л.';
        case Measure.tablespoon:
            return 'ст.л.';
        case Measure.glass:
            return 'стакан';
        default:
            return '';
    }
}

export function transformCreatedAt(dateString: string): string {
    if (!dateString || dateString.startsWith('0001-01-01')) {
        return 'Дата не указана';
    }

    const date = parseISO(dateString);
    return `Опубликована ${format(date, 'd MMMM yyyy', { locale: ru })}`;
}

// Универсальная функция для преобразования строки в значение enum
export function getEnumValueByString<T extends Record<string, string>>(
    enumObj: T,
    value: string
): T[keyof T] | undefined {
    return (Object.values(enumObj) as string[]).includes(value)
        ? (value as T[keyof T])
        : undefined;
}
