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
        case Category.dessert:
            return 'Десерт';
        case Category.dinner:
            return 'Обед';
        case Category.drinks:
            return 'Напиток';
        case Category.hot:
            return 'Горячее';
        case Category.salad:
            return 'Салат';
        case Category.sauce:
            return 'Соус';
        case Category.soups:
            return 'Суп';
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
