import { CATEGORY, DIFFICULTY, TAG } from 'mocks/filter';
import styles from './content.module.scss';
import { FilterList } from './filter-list';
import { useDispatch, useSelector } from 'store/store';
import { selectFilter, updateFilter } from 'store/main-page.slice';

export function FilterContent() {
    const dispatch = useDispatch();
    const filter = useSelector(selectFilter);

    return (
        <div className={styles.mainContainer}>
            <div className={styles.time}>
                <p className={styles.title}>Время приготовления</p>
                <p className={styles.filter}>
                    От
                    <input
                        type='text'
                        className={styles.input_filter}
                        value={filter.CookingTimeFrom}
                        onChange={(e) =>
                            dispatch(
                                updateFilter({
                                    CookingTimeFrom: +e.target.value,
                                })
                            )
                        }
                    />{' '}
                    мин до{' '}
                    <input
                        type='text'
                        className={styles.input_filter}
                        value={filter.CookingTimeTo}
                        onChange={(e) =>
                            dispatch(
                                updateFilter({ CookingTimeTo: +e.target.value })
                            )
                        }
                    />{' '}
                    мин
                </p>
            </div>
            <FilterList
                name='Сложность'
                list={DIFFICULTY}
                selected={filter.CookingDifficulties}
                onChange={(selected) =>
                    dispatch(updateFilter({ CookingDifficulties: selected }))
                }
            />
            <FilterList
                name='Категория'
                list={CATEGORY}
                selected={filter.Categories}
                onChange={(selected) =>
                    dispatch(updateFilter({ Categories: selected }))
                }
            />
            <FilterList
                name='Другое'
                list={TAG}
                selected={filter.Tags}
                onChange={(selected) =>
                    dispatch(updateFilter({ Tags: selected }))
                }
            />
        </div>
    );
}
