/* eslint-disable @typescript-eslint/no-non-null-assertion */
import {
    Search,
    SortContent,
    SortDirection,
    SortField,
} from 'components/filter';
import styles from './filter.module.scss';
import { ButtonLayoutWithoutButton } from 'components/filter/button-layout-wbutton';
type props<T extends SortField> = {
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    value?: string;
    onClick?: () => void;
    isFindShows?: boolean;
    selectedField: T;
    selectedDirection: SortDirection;
    onSortChange: (field: T, direction: SortDirection) => void;
    isCollection?: boolean;
};

export function FilterShort<T extends SortField>(props: props<T>) {
    const {
        onChange,
        value,
        onClick,
        isFindShows = true,
        selectedField,
        selectedDirection,
        onSortChange,
        isCollection,
    } = props;
    return (
        <div className={styles.mainContainer}>
            <Search
                onChange={onChange!}
                value={value!}
                onClick={onClick!}
                isFindShows={isFindShows}
            />
            <div className={styles.buttons}>
                <ButtonLayoutWithoutButton
                    icon='sort'
                    name='Сортировать'
                    color='white'
                >
                    <SortContent<T>
                        selectedField={selectedField}
                        selectedDirection={selectedDirection}
                        onChange={onSortChange}
                        isCollection={isCollection}
                    />
                </ButtonLayoutWithoutButton>
            </div>
        </div>
    );
}
