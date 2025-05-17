import { CollList } from 'components/collections';
import styles from './my-collections.module.scss';
import { useSelector } from 'store/store';
import { selectUserCollections } from 'store/user.slice';
import ScrollToTop from 'components/common/scrollToTop';
import { useState } from 'react';
import { SortDirection, SortFieldCollection } from 'components/filter';

function MyCollectionsPage() {
    const myCollections = useSelector(selectUserCollections);
    const [filter, setFilter] = useState('');
    const [sortField, setSortField] = useState<SortFieldCollection>('byCreated');
    const [sortDirection, setSortDirection] = useState<SortDirection>('desc');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFilter(e.target.value);
    };

    const handleSortChange = (
        field: SortFieldCollection,
        direction: SortDirection
    ) => {
        setSortField(field);
        setSortDirection(direction);
    };

    const sortedCollections = [...myCollections].sort((a, b) => {
        let comparison = 0;

        if (sortField === 'byPopularity') {
            comparison = a.popularity - b.popularity;
        } else if (sortField === 'byCreated') {
            comparison =
                new Date(a.createdAt).getTime() -
                new Date(b.createdAt).getTime();
        }

        return sortDirection === 'asc' ? comparison : -comparison;
    });

    const filteredCollections = sortedCollections.filter((collection) =>
        collection.title.toLowerCase().includes(filter.toLowerCase())
    );

    return (
        <>
            <ScrollToTop />
            <div className={styles.mainContainer}>
                <CollList
                    title='Мои подборки'
                    collections={filteredCollections}
                    value={filter}
                    onChange={handleChange}
                    isFindShows={false}
                    selectedField={sortField}
                    selectedDirection={sortDirection}
                    onSortChange={handleSortChange}
                />
            </div>
        </>
    );
}

export default MyCollectionsPage;
