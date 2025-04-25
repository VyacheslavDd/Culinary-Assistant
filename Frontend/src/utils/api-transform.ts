import { recipeDifficulty } from 'types';

export function transformDifficulty(difficulty: recipeDifficulty) {
    switch (difficulty) {
        case 'easy':
            return 'Легко';
        case 'medium':
            return 'Средне';
        case 'hard':
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
