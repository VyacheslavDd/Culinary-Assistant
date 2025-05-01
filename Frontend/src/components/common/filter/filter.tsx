import { ButtonLayout, Search, SortContent } from 'components/filter';
import styles from './filter.module.scss';
type props = {
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    value?: string;
    onClick?: () => void;
    isFindShows?: boolean;
};

export function FilterShort(props: props) {
    const { onChange, value, onClick, isFindShows = true } = props;
    return (
        <div className={styles.mainContainer}>
            {/* eslint-disable-next-line @typescript-eslint/no-non-null-assertion */}
            <Search onChange={onChange!} value={value!} onClick={onClick!} isFindShows={isFindShows} />
            <div className={styles.buttons}>
                <ButtonLayout
                    icon='sort'
                    name='Сортировать'
                    color='white'
                    onClick={onClick!}
                >
                    <SortContent />
                </ButtonLayout>
            </div>
        </div>
    );
}
