import { CardCollMax } from '../card-max/card-coll-max';
import styles from './coll-list.module.scss';
import { ShortCollection } from 'types/short-collection.type';
import { FilterShort } from '../../../components/common/filter/filter';
import { SortDirection, SortFieldCollection } from '../../../components/filter';

type Props = {
    title: string;
    collections: ShortCollection[];
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    value?: string;
    onClick?: () => void;
    isFindShows?: boolean;
    selectedField: SortFieldCollection;
    selectedDirection: SortDirection;
    onSortChange: (
        field: SortFieldCollection,
        direction: SortDirection
    ) => void;
};

export function CollList(props: Props) {
    const {
        title,
        collections,
        onChange,
        value,
        onClick,
        isFindShows,
        selectedField,
        selectedDirection,
        onSortChange,
    } = props;
    return (
        <div className={styles.mainContainer}>
            <h2 className={styles.title}>{title}</h2>
            <FilterShort
                onChange={onChange}
                value={value}
                onClick={onClick}
                isFindShows={isFindShows}
                selectedField={selectedField}
                selectedDirection={selectedDirection}
                onSortChange={onSortChange}
                isCollection
            />
            <ul className={styles.list}>
                {collections.map((item) => (
                    <li className={styles.item} key={item.id}>
                        <CardCollMax data={item} />
                    </li>
                ))}
                {collections.length === 0 && <p>Ничего не найдено</p>}
            </ul>
        </div>
    );
}
