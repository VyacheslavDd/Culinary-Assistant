import { Best, CollList } from 'components/collections';
import styles from './collections.module.scss';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchCollections,
    fetchPopularCollections,
    selectCollections,
    selectFilter,
    selectPopularCollections,
    updateFilter,
} from 'store/collections-page.slice';
import { useEffect } from 'react';
import { SortFieldCollection, SortDirection } from 'components/filter';

function CollectionsPage() {
    const popularCollections = useSelector(selectPopularCollections);
    const collections = useSelector(selectCollections);
    const dispatch = useDispatch();
    const filter = useSelector(selectFilter);

    useEffect(() => {
        dispatch(fetchCollections());
        dispatch(fetchPopularCollections());
    }, [dispatch]);

    return (
        <div className={styles.mainContainer}>
            <Best collections={popularCollections} />
            <CollList
                title='Все подборки'
                collections={collections}
                onChange={(e) =>
                    dispatch(updateFilter({ Title: e.target.value }))
                }
                value={filter.Title}
                onClick={() => dispatch(fetchCollections())}
                selectedField={'byPopularity'}
                selectedDirection={'asc'}
                onSortChange={function (
                    field: SortFieldCollection,
                    direction: SortDirection
                ): void {
                    throw new Error('Function not implemented.');
                }}
            />
        </div>
    );
}

export default CollectionsPage;
