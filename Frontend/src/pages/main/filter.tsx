import {
    ButtonLayout,
    FilterContent,
    IngredientsContent,
    Search,
    SortContent,
} from 'components/filter';
import styles from './filter.module.scss';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchRecipes,
    selectFilter,
    updateFilter,
} from 'store/main-page.slice';

export function Filter() {
    const filter = useSelector(selectFilter);
    const dispatch = useDispatch();

    return (
        <div className={styles.mainContainer}>
            <Search
                onChange={(e) => {
                    dispatch(updateFilter({ SearchByTitle: e.target.value }));
                }}
                onClick={() => {
                    dispatch(fetchRecipes());
                }}
                value={filter.SearchByTitle || ''}
            />
            <div className={styles.buttons}>
                {' '}
                <ButtonLayout
                    icon='ingredients'
                    name='Ингредиенты'
                    color='blue'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <IngredientsContent />
                </ButtonLayout>
                <ButtonLayout
                    icon='filter'
                    name='Фильтровать'
                    color='white'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <FilterContent />
                </ButtonLayout>
                <ButtonLayout
                    icon='sort'
                    name='Сортировать'
                    color='white'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <SortContent />
                </ButtonLayout>
            </div>
        </div>
    );
}
