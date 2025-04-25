import { ButtonLayout, FilterContent, IngredientsContent, Search, SortContent } from 'components/filter';
import styles from './filter.module.scss';

export function Filter() {
    return (
        <div className={styles.mainContainer}>
            <Search />
            <div className={styles.buttons}>
                {' '}
                <ButtonLayout
                    icon='ingredients'
                    name='Ингредиенты'
                    color='blue'
                >
                    <IngredientsContent />
                </ButtonLayout>
                <ButtonLayout icon='filter' name='Фильтровать' color='white'>
                    <FilterContent />
                </ButtonLayout>
                <ButtonLayout icon='sort' name='Сортировать' color='white'>
                    <SortContent />
                </ButtonLayout>
            </div>
        </div>
    );
}
